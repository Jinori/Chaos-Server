using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public sealed class SmallStunEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 364
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(3);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override byte Icon => 97;
    /// <inheritdoc />
    public override string Name => "Small Stun";

    public override void OnApplied()
    {
        if (Subject is not Monster monster)
            return;

        if (!Subject.Status.HasFlag(Status.BeagSuain))
            Subject.Status = Status.BeagSuain;
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => AislingSubject?.Client.SendCancelCasting();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.BeagSuain))
            Subject.Status &= ~Status.BeagSuain;
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if ((target.Status & Status.BeagSuain) != 0)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already stunned.");

            return false;
        }

        return true;
    }
}