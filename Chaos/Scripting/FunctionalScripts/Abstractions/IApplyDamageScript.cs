using Chaos.Formulae.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IApplyDamageScript : IFunctionalScript
{
    IDamageFormula DamageFormula { get; set; }

    void ApplyDamage(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    );

    static virtual IApplyDamageScript Create() => null!;
}