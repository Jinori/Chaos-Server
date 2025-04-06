using Chaos.Collections;
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
        CleanupTimer = new IntervalTimer(TimeSpan.FromSeconds(600), false);
    }

    public override void Update(TimeSpan delta)
    {
        AislingDeathTouchTimer.Update(delta);
        CleanupTimer.Update(delta);

        if (CleanupTimer.IntervalElapsed)
        {
            HandleRecentTalkers();
            RemoveFromGroup();
        }

        if (AislingDeathTouchTimer.IntervalElapsed)
        {
            WarpGhostsToPoint();
            HandleAislingTouchDeath();
        }
    }

    private void HandleRecentTalkers()
    {
        var recentlyTalked = Subject.GetEntities<Aisling>()
                                    .Where(aisling => aisling.Trackers.LastTalk.HasValue &&
                                                      (DateTime.UtcNow - aisling.Trackers.LastTalk.Value).TotalSeconds <= 1)
                                    .ToList();

        if (recentlyTalked.Count > 1)
        {
            foreach (var aisling in recentlyTalked)
            {
                ApplyDamageScript.ApplyDamage(aisling, aisling, this, 500_000);
                aisling.SendMessage("Shhh. Don't speak or die.");
                Subject.ShowAnimation(_blowupAnimation.GetPointAnimation(aisling));
            }
        }
    }

    private void WarpGhostsToPoint()
    {
        var ghosts = Subject.GetEntities<Aisling>()
                            .Where(aisling =>
                                (aisling.IsDead && (aisling.X != _ghostPoint.X || aisling.Y != _ghostPoint.Y)) ||
                                !aisling.IsAlive);

        foreach (var ghost in ghosts)
            ghost.WarpTo(_ghostPoint);
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
                                .Where(x => x.IsAlive)
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
