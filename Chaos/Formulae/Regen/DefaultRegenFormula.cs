using Chaos.Common.Utilities;
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

        if (creature.Effects.Contains("Poison") || creature.Effects.Contains("miasma"))
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
        else
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
    public int CalculateIntervalSecs(Creature creature) => 6;

    /// <inheritdoc />
    public int CalculateManaRegen(Creature creature)
    {
        if (creature.StatSheet.ManaPercent == 100)
            return 0;

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