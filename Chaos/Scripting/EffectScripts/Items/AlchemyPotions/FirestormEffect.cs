using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class FirestormEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 60
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(12);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override byte Icon => 100;
    /// <inheritdoc />
    public override string Name => "Firestorm";

    protected void OnApply()
    {
        if (Subject is not Monster monster)
            return;

        Subject.StatSheet.TrySubtractHp(1500);
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject is not Monster monster)
            return;

        const int DAMAGE_PER_TICK = 100;

        if (Subject.StatSheet.CurrentHp <= DAMAGE_PER_TICK)
            return;

        if (Subject.StatSheet.TrySubtractHp(DAMAGE_PER_TICK))
        {
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.ShowHealth();
        }
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Small Firestorm")
            || target.Effects.Contains("Firestorm")
            || target.Effects.Contains("Firepunch"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already burning.");

            return false;
        }

        return true;
    }
}