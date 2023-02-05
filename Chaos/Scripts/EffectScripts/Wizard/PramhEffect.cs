using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Wizard;

public sealed class PramhEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 90;
    /// <inheritdoc />
    public override string Name => "pramh";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 28
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(12);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your eyelids become heavy.");

        if (!Subject.Status.HasFlag(Status.Pramh))
            Subject.Status = Status.Pramh;
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => AislingSubject?.Client.SendCancelCasting();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.Pramh))
            Subject.Status &= ~Status.Pramh;

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You awake from your slumber.");
    }
}