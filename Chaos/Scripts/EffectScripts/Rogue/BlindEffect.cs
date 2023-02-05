using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Rogue;

public sealed class BlindEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 3;
    /// <inheritdoc />
    public override string Name => "Blind";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 75,
        TargetAnimation = 42
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(20);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are unable to see.");

        if (!Subject.Status.HasFlag(Status.Blind))
            Subject.Status = Status.Blind;
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() { }

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.Blind))
            Subject.Status &= ~Status.Blind;

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can see again.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Blind"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Blind has already been applied.");

            return false;
        }

        return true;
    }
}