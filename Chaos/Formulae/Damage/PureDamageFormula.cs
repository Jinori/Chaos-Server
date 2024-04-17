using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
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
        // Check if the source (attacker) has godmode enabled.
        var isGodModeEnabled = target is Aisling aisling && aisling.Trackers.Enums.HasValue(GodMode.Yes);

        // If godmode is enabled, set the damage to 0.
        if (!isGodModeEnabled) 
            return damage;
        
        damage = 0;
        return damage;

    }
}