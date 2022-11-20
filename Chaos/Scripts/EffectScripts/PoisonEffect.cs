using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts;

public sealed class PoisonEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 20;
    /// <inheritdoc />
    public override string Name => "Poison";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 25
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(10);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    public override void OnApplied(Creature target)
    {
        Subject = target;
        AislingSubject = target as Aisling;
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel a creeping feeling of death.");
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel fine again.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        Subject?.StatSheet.SubtractHealthPct(5);
        //if the subject was a player, update their vit
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}