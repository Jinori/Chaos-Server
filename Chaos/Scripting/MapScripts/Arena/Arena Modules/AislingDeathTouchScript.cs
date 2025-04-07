using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class AislingDeathTouchScript : MapScriptBase
{
    private bool _aislingsTouching;
    private Aisling? _touchOne;
    private Aisling? _touchTwo;
    private readonly HashSet<Aisling> WarpedGhosts = [];
    private IIntervalTimer AislingDeathTouchTimer { get; }
    private IIntervalTimer CleanupTimer { get; }
    private IApplyDamageScript ApplyDamageScript { get; }

    private readonly Animation _blowupAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 49
    };

    private readonly Point _ghostPoint = new(20, 10);

    public AislingDeathTouchScript(MapInstance subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        AislingDeathTouchTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);
        CleanupTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
    }

    public override void Update(TimeSpan delta)
    {
        AislingDeathTouchTimer.Update(delta);
        CleanupTimer.Update(delta);

        if (CleanupTimer.IntervalElapsed)
        {
            HandleRecentActions();
            RemoveFromGroup();
        }

        if (AislingDeathTouchTimer.IntervalElapsed)
        {
            WarpGhostsToPoint();
            HandleAislingTouchDeath();
        }
    }

    private void HandleRecentActions()
    {
        var now = DateTime.UtcNow;

        var violators = Subject.GetEntities<Aisling>()
                               .Where(aisling => !WarpedGhosts.Contains(aisling))
                               .Select(aisling => new
                               {
                                   Aisling = aisling,
                                   Talked = aisling.Trackers.LastTalk.HasValue &&
                                            ((now - aisling.Trackers.LastTalk.Value).TotalSeconds <= 5),

                                   CastSpell = aisling.Trackers.LastSpellUse.HasValue &&
                                               ((now - aisling.Trackers.LastSpellUse.Value).TotalSeconds <= 5),

                                   UsedSkill = aisling.Trackers.LastSkillUse.HasValue &&
                                               ((now - aisling.Trackers.LastSkillUse.Value).TotalSeconds <= 5)
                               })
                               .Where(x => x.Talked || x.CastSpell || x.UsedSkill)
                               .ToList();

        foreach (var offender in violators)
        {
            var aisling = offender.Aisling;
            ApplyDamageScript.ApplyDamage(aisling, aisling, this, 500000);
            Subject.ShowAnimation(_blowupAnimation.GetPointAnimation(aisling));

            if (offender.Talked)
                aisling.SendMessage("Shhh. You shouldn't speak!");
            else if (offender.CastSpell)
                aisling.SendMessage("Don't cast spells!");
            else if (offender.UsedSkill)
                aisling.SendMessage("Don't use skills!");
        }
    }


    private void WarpGhostsToPoint()
    {
        var ghosts = Subject.GetEntities<Aisling>()
                            .Where(aisling =>
                                aisling.IsDead &&
                                !WarpedGhosts.Contains(aisling))
                            .ToList();

        foreach (var ghost in ghosts)
        {
            ghost.WarpTo(_ghostPoint);
            WarpedGhosts.Add(ghost);
        }
    }

    private void RemoveFromGroup()
    {
        var grouped = Subject.GetEntities<Aisling>()
                             .Where(x => x.Group != null);

        foreach (var aisling in grouped)
        {
            aisling.Group?.Kick(aisling);
            aisling.SendMessage("Removed from group to prevent seeing group members.");
        }
    }

    private void HandleAislingTouchDeath()
    {
        if (!_aislingsTouching)
        {
            var living = Subject.GetEntities<Aisling>()
                                .Where(x => x.IsAlive && !x.IsGodModeEnabled())
                                .ToList();

            foreach (var aisling in living)
            {
                var other = living.FirstOrDefault(other =>
                    !other.Equals(aisling) && (aisling.ManhattanDistanceFrom(other) <= 1));

                if (other != null)
                {
                    _touchOne = aisling;
                    _touchTwo = other;
                    _aislingsTouching = true;
                    break;
                }
            }
        }
        else if ((_touchOne != null) && (_touchTwo != null))
        {
            ApplyDamageScript.ApplyDamage(_touchOne, _touchTwo, this, 500000);
            ApplyDamageScript.ApplyDamage(_touchTwo, _touchOne, this, 500000);

            Subject.ShowAnimation(_blowupAnimation.GetPointAnimation(_touchOne));
            Subject.ShowAnimation(_blowupAnimation.GetPointAnimation(_touchTwo));

            _touchOne = null;
            _touchTwo = null;
            _aislingsTouching = false;
        }
    }
}
