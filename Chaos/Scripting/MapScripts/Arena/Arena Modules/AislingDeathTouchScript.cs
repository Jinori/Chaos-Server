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
    private bool AislingsTouching;
    private Aisling? TouchOne;
    private Aisling? TouchTwo;
    private IIntervalTimer AislingDeathTouchTimer { get; }
    private IApplyDamageScript ApplyDamageScript { get; }

    private Animation BlowupAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 49
    };

    /// <inheritdoc />
    public AislingDeathTouchScript(MapInstance subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        AislingDeathTouchTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        AislingDeathTouchTimer.Update(delta);

        if (!AislingDeathTouchTimer.IntervalElapsed)
            return;

        foreach (var ghost in Subject.GetEntities<Aisling>()
                                     .Where(x => x.IsDead || !x.IsAlive))
            ghost.WarpTo(new Point(20, 10));

        foreach (var person in Subject.GetEntities<Aisling>()
                                      .Where(x => x.Group != null))
        {
            person.Group?.Kick(person);
            person.SendMessage("Removed from group to prevent seeing group members.");
        }

        if (!AislingsTouching)
        {
            var aislings = Subject.GetEntities<Aisling>()
                                  .Where(x => x.IsAlive)
                                  .ToList();

            foreach (var aisling in aislings)
            {
                var otherAisling = aislings.FirstOrDefault(other => !other.Equals(aisling) && (aisling.ManhattanDistanceFrom(other) <= 1));

                if (otherAisling != null)
                {
                    TouchOne = aisling;
                    TouchTwo = otherAisling;
                    AislingsTouching = true;

                    break;
                }
            }
        } else
        {
            if ((TouchOne != null) && (TouchTwo != null))
            {
                ApplyDamageScript.ApplyDamage(
                    TouchOne,
                    TouchTwo,
                    this,
                    500000);

                ApplyDamageScript.ApplyDamage(
                    TouchTwo,
                    TouchOne,
                    this,
                    500000);

                Subject.ShowAnimation(BlowupAnimation.GetPointAnimation(TouchOne));
                Subject.ShowAnimation(BlowupAnimation.GetPointAnimation(TouchTwo));

                TouchOne = null;
                TouchTwo = null;
                AislingsTouching = false;
            }
        }
    }
}