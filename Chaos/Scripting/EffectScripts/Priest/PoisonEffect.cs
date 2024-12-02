using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class PoisonEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 247,
        Priority = 70
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 27;

    /// <inheritdoc />
    public override string Name => "Poison";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        double maxHp = Subject.StatSheet.MaximumHp;
        const double DAMAGE_PERCENTAGE = 0.01;
        const int DAMAGE_CAP = 500;

        var damage = (int)Math.Min(maxHp * DAMAGE_PERCENTAGE, DAMAGE_CAP);

        if (Subject.StatSheet.CurrentHp <= damage)
            return;

        if (Subject.IsGodModeEnabled() || Subject.Effects.Contains("Invulnerability"))
        {
            Subject.Effects.Terminate("Poison");

            return;
        }

        if (Subject.StatSheet.TrySubtractHp(damage))
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);

        Subject.Animate(Animation);
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.IsGodModeEnabled())
            return false;

        if (target.Effects.Contains("Poison Immunity"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is currently immune to poison.");

            return false;
        }

        if (target.Effects.Contains("Poison"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target has this effect already.");

            return false;
        }

        if (target.Effects.Contains("Miasma"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is currently affected by Miasma.");

            return false;
        }

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");;
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }
}