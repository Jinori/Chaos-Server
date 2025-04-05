using Chaos.Formulae.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Healing;

public class DefaultHealFormula : IHealFormula
{
    /// <inheritdoc />
    public virtual int Calculate(
        Creature source,
        Creature target,
        IScript script,
        int healing)
    {
        ApplyHealingBonuses(source, ref healing);

        return healing;
    }

    private void ApplyHealingBonuses(Creature source, ref int healing)
    {
        if (healing <= 0)
        {
            healing = 0;

            return;
        }

        // Percent heal bonus
        if (source.StatSheet.EffectiveHealBonusPct > 0)
        {
            var bonusPct = source.StatSheet.EffectiveHealBonusPct / 100m;
            healing += (int)(healing * bonusPct);
        }
        
        // Flat heal bonus
        if (source.StatSheet.EffectiveHealBonus > 0)
            healing += source.StatSheet.EffectiveHealBonus;
    }
}