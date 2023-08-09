using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Jobs;

public sealed class SmashVialHealEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 126
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(10);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));
    /// <inheritdoc />
    public override byte Icon => 145;
    /// <inheritdoc />
    public override string Name => "HealthRegen";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        //the interval is 100ms, so this will be applied 10 times a second
        const int healPerTick = 5;
        Subject.StatSheet.AddHp(healPerTick);
        //if the subject was a player, update their vit
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}