using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public sealed class DallEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 75,
        TargetAnimation = 42
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(25);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override byte Icon => 2;
    /// <inheritdoc />
    public override string Name => "Blind";

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
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "That target is already blinded.");

            return false;
        }

        return true;
    }
}