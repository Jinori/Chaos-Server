using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Monk;

public sealed class WolfFangFistEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 50;
    /// <inheritdoc />
    public override string Name => "wolfFangFist";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 40
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(6);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A deadly strike puts you to sleep.");

        if (!Subject.Status.HasFlag(Status.Suain))
            Subject.Status = Status.Suain;
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => AislingSubject?.Client.SendCancelCasting();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.Suain))
            Subject.Status &= ~Status.Suain;

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel fine again.");
    }
}