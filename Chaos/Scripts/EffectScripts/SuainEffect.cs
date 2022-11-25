using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts;

public sealed class SuainEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 50;
    /// <inheritdoc />
    public override string Name => "Suain";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 40
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel ice run through your veins.");
        if (!Subject.Status.HasFlag(Status.Suain))
            Subject.Status = Status.Suain;  
    }

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.Suain))
            Subject.Status &= ~Status.Suain;

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel fine again.");
    }


    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        AislingSubject?.Client.SendCancelCasting();
    }
}