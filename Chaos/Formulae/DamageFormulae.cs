using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Damage;

namespace Chaos.Formulae;

public static class DamageFormulae
{
    public static DefaultDamageFormula Default => new();
    public static PureDamageFormula PureDamage => new();
    public static ElementalEffectDamageFormula ElementalEffect => new();
}