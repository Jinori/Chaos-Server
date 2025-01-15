using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Formulae.Damage;

public class PureDamageFormula : IDamageFormula
{
    /// <inheritdoc />
    public virtual int Calculate(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null)
    {
        if (target.IsGodModeEnabled())
            return 0;

        if (target.Name == "Shamensyth")
            if ((elementOverride ?? source.StatSheet.OffenseElement) == Element.Fire)
            {
                if (source is Aisling aisling)
                    aisling.SendOrangeBarMessage("Shamensyth is immune to the fire!");

                return 0;
            }

        return damage;
    }
}