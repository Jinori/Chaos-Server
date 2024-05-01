using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public sealed class EarthPunchEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(2);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 70,
        TargetAnimation = 208
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override byte Icon => 46;
    /// <inheritdoc />
    public override string Name => "earthpunch";

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "After taking a strike, you feel as though you cannot move.");

        if (!Subject.Status.HasFlag(Status.BeagSuain))
            Subject.Status = Status.BeagSuain;
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => AislingSubject?.Client.SendCancelCasting();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.BeagSuain))
            Subject.Status &= ~Status.BeagSuain;

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel fine again.");
    }
}