using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public sealed class BeagSuainEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 364
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override byte Icon => 117;
    /// <inheritdoc />
    public override string Name => "BeagSuain";

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

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Beag Suain")
            || target.Effects.Contains("Suain")
            || target.Effects.Contains("SmallStun")
            || target.Effects.Contains("Stun"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already stunned.");

            return false;
        }

        return true;
    }
}