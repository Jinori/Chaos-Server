using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    /// <inheritdoc />
    public int CalculateHealthRegen(Creature creature)
    {
        if (creature.StatSheet.HealthPercent == 100)
            return 0;

        if (creature.Effects.Contains("Poison") || creature.Effects.Contains("Miasma"))
            return 0;

        if (creature.IsInnerFired())
        {
            var percentToRegenerate = creature switch
            {
                Aisling  => 18,
                Monster  => 11,
                Merchant => 100,
                _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
            };

            return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, percentToRegenerate);
        }

        if (creature.IsHotChocolated())
        {
            var percentToRegenerate = creature switch
            {
                Aisling  => 16,
                Monster  => 11,
                Merchant => 100,
                _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
            };

            return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, percentToRegenerate);
        } else
        {
            var percentToRegenerate = creature switch
            {
                Aisling  => 10,
                Monster  => 3,
                Merchant => 100,
                _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
            };

            return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, percentToRegenerate);
        }
    }

    /// <inheritdoc />
    public int CalculateIntervalSecs(Creature creature)
    {
        // Ensure creature and its StatSheet are not null
        if ((creature == null) || (creature.StatSheet == null))
            return 12;

        // Base interval time in seconds
        const int BASE_INTERVAL = 16;

        // Minimum interval time
        const int MIN_INTERVAL = 6;

        // Reduction rate: how much interval decreases per point of Con
        const double REDUCTION_PER_CON = 10.0 / 255;

        if (creature.StatSheet.Con > 3)
        {
            var con = creature.StatSheet.GetEffectiveStat(Stat.CON);

            // Calculate the reduced interval
            var reducedInterval = BASE_INTERVAL - con * REDUCTION_PER_CON;

            // Ensure the interval is not lower than the minimum interval
            return Math.Max((int)Math.Ceiling(reducedInterval), MIN_INTERVAL);
        }

        // Ensure the interval is not lower than the minimum interval
        return 12;
    }

    /// <inheritdoc />
    public int CalculateManaRegen(Creature creature)
    {
        if (creature.StatSheet.ManaPercent == 100)
            return 0;

        if (creature.IsHotChocolated())
        {
            var percentToRegenerate = creature switch
            {
                Aisling  => 8,
                Monster  => 2,
                Merchant => 100,
                _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
            };

            return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, percentToRegenerate);
        } else
        {
            var percentToRegenerate = creature switch
            {
                Aisling  => 5,
                Monster  => 1.5m,
                Merchant => 100,
                _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
            };

            return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, percentToRegenerate);
        }
    }
}