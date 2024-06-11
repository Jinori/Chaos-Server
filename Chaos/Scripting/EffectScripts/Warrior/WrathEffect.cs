using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class WrathEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 95
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();
    /// <inheritdoc />
    public override byte Icon => 98;
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override string Name => "Wrath";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.StatSheet.EffectiveMaximumMp <= 99)
        {
            if (Subject.StatSheet.CurrentMp <= 1)
            {
                Subject.Effects.Terminate(Name);
                return;
            }

            Subject.StatSheet.SubtractMp(10);
        }
        else
        {
            if ((Subject.StatSheet.ManaPercent <= 5m) || (Subject.StatSheet.CurrentMp <= 1))
            {
                Subject.Effects.Terminate(Name);
                return;
            }

            Subject.StatSheet.SubtractManaPct(5m);
        }
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);

        var points = AoeShape.AllAround.ResolvePoints(Subject);

        var targets =
            Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>()).WithFilter(Subject, TargetFilter.HostileOnly).ToList();

        var damage = (Subject.StatSheet.EffectiveStr + 3) * 2;
        
        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(
                Subject,
                target,
                this,
                damage);

            target.ShowHealth();
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "You end your wrath and your skin turns to normal.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.StatSheet.ManaPercent <= 5m)
            return false;

        return true;
    }
}
