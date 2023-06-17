using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class FirePunchEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 35;
    /// <inheritdoc />
    public override string Name => "firepunch";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 60
    };
    /// <inheritdoc />
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(2);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var damagePerTick = 5 * Subject.StatSheet.Level;

        if (Subject.StatSheet.CurrentHp <= damagePerTick)
            return;

        if (Subject.StatSheet.TrySubtractHp(damagePerTick))
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}