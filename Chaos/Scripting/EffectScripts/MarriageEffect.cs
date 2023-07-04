using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class MarriageEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 150,
        TargetAnimation = 36
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(2);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));
    /// <inheritdoc />
    public override byte Icon => 152;
    /// <inheritdoc />
    public override string Name => "Marriage";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        //const int DAMAGE_PER_TICK = 5;

        //if (Subject.StatSheet.CurrentHp <= DAMAGE_PER_TICK)
        //return;

        //Subject.StatSheet.SubtractHp(DAMAGE_PER_TICK);
        //AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}