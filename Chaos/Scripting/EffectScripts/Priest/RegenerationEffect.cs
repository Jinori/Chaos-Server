using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class RegenerationEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

    private Creature SourceOfEffect { get; set; } = null!;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 125
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

    protected IApplyHealScript ApplyHealScript { get; }

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 98;

    /// <inheritdoc />
    public override string Name => "Regeneration";

    public RegenerationEffect() => ApplyHealScript = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        // Step 1: Calculate the base heal amount using the source's WIS and 1% of the target's max HP
        var wis = SourceOfEffect.StatSheet.Wis;
        var targetMaxHp = Subject.StatSheet.EffectiveMaximumHp;
        var baseHealAmount = wis * 5 + targetMaxHp / 100; // WIS multiplier + 1% of target's max HP

        // Step 2: Add the source's EffectiveHealBonus to the base heal amount
        var healBonus = SourceOfEffect.StatSheet.EffectiveHealBonus;
        var totalHealWithBonus = baseHealAmount + healBonus;

        // Step 3: Multiply the result by the source's EffectiveHealBonusPct
        var healBonusPct = SourceOfEffect.StatSheet.EffectiveHealBonusPct / 100f;
        var finalHealAmount = totalHealWithBonus * (1 + healBonusPct); // Add 1 to include the base amount

        // Apply the calculated heal to the target
        ApplyHealScript.ApplyHeal(
            SourceOfEffect,
            Subject,
            ApplyHealScript,
            (int)finalHealAmount);

        // Animate the target
        Subject.Animate(Animation);
    }

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Regeneration has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        if (target.Effects.Contains("Regeneration"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your ally has Regeneration already.");

            return false;
        }

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name}.");
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }
}