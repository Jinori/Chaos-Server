using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Damage;

public class PureDamageFormula : IDamageFormula
{
    /// <inheritdoc />
    public virtual int Calculate(
        Creature? source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null)
    {
        if (target.IsGodModeEnabled())
            return 0;

        return damage;

    }
}