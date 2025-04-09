using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.EffectScripts.Priest.Shadow;

public class DarkStormEffect : ContinuousAnimationEffectBase
{
    /// <summary>
    ///     Adjust the scaling damage multiplier based on caster's stats or other factors.
    /// </summary>
    private const double DAMAGE_SCALING_FACTOR = 0.25;

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(25);

    private Creature SourceOfEffect { get; set; } = null!;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 76 // Animation for the storm effect
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    private IApplyDamageScript ApplyDamageScript { get; } = ApplyNonAttackDamageScript.Create();

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    public override byte Icon => 125;

    public override string Name => "DarkStorm";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var nonWallPoints = Subject.SpiralSearch(2)
                                   .Where(x => !Subject.MapInstance.IsWall(x));

        // Apply the effect to multiple nearby targets instead of one random point
        foreach (var point in nonWallPoints)
        {
            var targets = Subject.MapInstance
                                 .GetEntitiesAtPoints<Creature>(point)
                                 .Where(x => x is not Merchant && x.IsAlive && x.IsHostileTo(Subject))
                                 .ToList();

            if (targets.Count <= 0)
                continue;

            foreach (var target in targets)
            {
                // Animation for storm strike on the target
                Subject.MapInstance.ShowAnimation(Animation.GetPointAnimation(point));

                // Damage calculation
                var damage = (int)(Subject.StatSheet.EffectiveInt * DAMAGE_SCALING_FACTOR);
                var pctDmg = DamageHelper.CalculatePercentDamage(Source, Subject, 2);

                damage = Math.Max(damage, pctDmg);

                ApplyDamageScript.ApplyDamage(
                    SourceOfEffect,
                    target,
                    this,
                    damage);
            }
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The chaos subsides.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        return true;
    }
}