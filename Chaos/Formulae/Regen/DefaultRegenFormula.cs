using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    private const decimal BASE_HP_REGEN_PERCENT = 6m; // Base % regeneration
    private const decimal BASE_MP_REGEN_PERCENT = 4m; // Base % regeneration
    private const decimal MAX_HP_REGEN_PERCENT = 18m; // Max % regeneration at cap stats
    private const decimal MAX_MP_REGEN_PERCENT = 16m; // Max % regeneration at cap stats
    private const decimal MAX_CON_BONUS = 8m; // Max % from Constitution
    private const decimal MAX_WIS_BONUS = 8m; // Max % from Wisdom
    private const decimal INNER_FIRE_BONUS = 4m; // Bonus % from InnerFire
    private const decimal HOT_CHOCOLATE_HEALTH_BONUS = 6m; // Bonus % health regen from HotChocolate
    private const decimal HOT_CHOCOLATE_MANA_BONUS = 3m; // Bonus % mana regen from HotChocolate
    private const int MAX_STAT = 200; // Maximum value for stats (Con/Wis)

    public int CalculateHealthRegen(Creature creature)
    {
        if (creature.StatSheet.HealthPercent == 100)
            return 0;

        if (creature.Effects.Contains("Poison") || creature.Effects.Contains("Miasma"))
            return 0;

        if (creature is Monster)
            return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, 1.5m);

        // Base health regeneration %
        var healthRegenPercent = BASE_HP_REGEN_PERCENT;

        // Add Constitution-based bonus
        var con = creature.StatSheet.GetEffectiveStat(Stat.CON);
        healthRegenPercent += con / (decimal)MAX_STAT * MAX_CON_BONUS;

        // Apply InnerFire bonus
        if (creature.IsInnerFired())
            healthRegenPercent += INNER_FIRE_BONUS;

        // Apply HotChocolate bonus
        if (creature.IsHotChocolated())
            healthRegenPercent += HOT_CHOCOLATE_HEALTH_BONUS;

        // Clamp to max regeneration cap
        healthRegenPercent = Math.Min(healthRegenPercent, MAX_HP_REGEN_PERCENT);

        // Calculate regeneration value based on effective max HP
        return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, healthRegenPercent);
    }

    public int CalculateIntervalSecs(Creature creature) => 8;

    public int CalculateManaRegen(Creature creature)
    {
        if (creature.StatSheet.ManaPercent == 100)
            return 0;

        // Base mana regeneration %
        var manaRegenPercent = BASE_MP_REGEN_PERCENT;

        // Add Wisdom-based bonus
        var wis = creature.StatSheet.GetEffectiveStat(Stat.WIS);
        manaRegenPercent += wis / (decimal)MAX_STAT * MAX_WIS_BONUS;

        // Apply HotChocolate bonus
        if (creature.IsHotChocolated())
            manaRegenPercent += HOT_CHOCOLATE_MANA_BONUS;

        // Clamp to max regeneration cap
        manaRegenPercent = Math.Min(manaRegenPercent, MAX_MP_REGEN_PERCENT);

        // Calculate regeneration value based on effective max MP
        return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, manaRegenPercent);
    }
}