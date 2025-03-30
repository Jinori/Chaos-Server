using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class InnerFireEffect : ContinuousAnimationEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(8);

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 300,
        TargetAnimation = 5
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMinutes(8), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(400), false);

    public override byte Icon => 65;
    public override string Name => "Inner Fire";

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body is at inner peace.");
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed() { }

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body returns to a normal state.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.Effects.Contains("Inner Fire"))
        {
            source.Effects.Dispel("Inner Fire");

            return true;
        }

        if (source.Effects.Contains("Hot Chocolate"))
        {
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body is already to warm.");

            return false;
        }

        return true;
    }
}