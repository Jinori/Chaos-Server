using System.Collections.Immutable;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Formulae.Damage;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DefaultDamageFormula : IDamageFormula
{
    private ImmutableArray<ImmutableArray<decimal>> ElementalModifierLookup { get; } = new[]
    {
        // @formatter:off
        //mostly lifted from http://da-wizard.com/elements.html
        //                                                  D E F E N S E
        //                         None,  Fire,  Water,  Wind,  Earth, Holy,  Darkness,  Wood,  Metal,  Undead
        /*      None*/     new[] { 0.58m, 0.37m, 0.37m,  0.37m, 0.37m, 0.37m, 0.37m,     0.37m, 0.37m,  0.37m }.ToImmutableArray(),
        /* O    Fire*/     new[] { 2.32m, 0.58m, 0.66m,  1.74m, 1.03m, 0.93m, 0.83m,     2.01m, 0.83m,  2.01m }.ToImmutableArray(),
        /* F    Water*/    new[] { 2.32m, 1.74m, 0.58m,  1.03m, 0.66m, 0.93m, 0.83m,     0.58m, 1.74m,  0.93m }.ToImmutableArray(), 
        /* F    Wind*/     new[] { 2.32m, 0.66m, 1.03m,  0.58m, 1.74m, 0.93m, 0.83m,     1.03m, 1.74m,  0.83m }.ToImmutableArray(),
        /* E    Earth*/    new[] { 2.32m, 1.03m, 1.74m,  0.66m, 0.58m, 0.93m, 0.83m,     0.58m, 0.83m,  0.58m }.ToImmutableArray(),
        /* N    Holy*/     new[] { 2.32m, 0.76m, 0.76m,  0.76m, 0.76m, 0.58m, 1.48m,     0.58m, 0.76m,  2.01m }.ToImmutableArray(),
        /* S    Darkness*/ new[] { 2.32m, 1.25m, 1.25m,  1.25m, 1.25m, 1.48m, 0.58m,     1.48m, 0.58m,  0.58m }.ToImmutableArray(),
        /* E    Wood*/     new[] { 2.32m, 0.58m, 0.76m,  1.03m, 1.74m, 0.93m, 0.83m,     0.58m, 1.03m,  0.83m }.ToImmutableArray(),
        /*      Metal*/    new[] { 2.32m, 0.83m, 0.50m,  1.88m, 0.83m, 0.93m, 0.83m,     1.25m, 0.58m,  1.03m }.ToImmutableArray(),
        /*      Undead*/   new[] { 2.32m, 0.50m, 0.83m,  0.83m, 1.88m, 0.93m, 0.83m,     0.58m, 0.76m,  0.58m }.ToImmutableArray() 
        // @formatter:on
    }.ToImmutableArray();

    protected virtual void ApplyAcModifier(ref int damage, int defenderAc) => damage = Convert.ToInt32(damage * (1 + defenderAc / 100.0m));

    protected virtual void ApplyElementalModifier(ref int damage, Element attackElement, Element defenseElement) =>
        damage = Convert.ToInt32(damage * ElementalModifierLookup[(int)attackElement][(int)defenseElement]);

    protected virtual void HandleElementalModifier(ref int damage, IScript source, Creature attacker, Creature defender)
    {
        switch (source)
        {
            case ISkillScript:
            {
                ApplyElementalModifier(ref damage, attacker.StatSheet.OffenseElement, defender.StatSheet.DefenseElement);
                break;
            }
            case ISpellScript:
            {
                ApplyElementalModifier(ref damage, attacker.StatSheet.OffensiveCastElement, defender.StatSheet.DefenseElement);
                break;
            }
        }
    }
    
    protected virtual void ApplySkillSpellModifier(ref int damage, IScript source, Creature attacker)
    {
        switch (source)
        {
            case ISkillScript:
            {
                var addedFromPct = damage * (attacker.StatSheet.EffectiveSkillDamagePct / 100);
                damage += attacker.StatSheet.EffectiveFlatSkillDamage + addedFromPct;

                break;
            }
            case ISpellScript:
            {
                var addedFromPct = damage * (attacker.StatSheet.EffectiveSpellDamagePct / 100);
                damage += attacker.StatSheet.EffectiveFlatSpellDamage + addedFromPct;

                break;
            }
        }
    }

    /// <inheritdoc />
    public int Calculate(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    )
    {
        ApplySkillSpellModifier(ref damage, source, attacker);

        var defenderAc = GetDefenderAc(defender);

        ApplyAcModifier(ref damage, defenderAc);
        HandleElementalModifier(ref damage, source, attacker, defender);
        HandleClawFist(ref damage, source, attacker);
        HandleZap(ref damage, source, attacker);
        return damage;
    }

    private void HandleZap(ref int damage, IScript source, Creature attacker)
    {
        if (source is not SubjectiveScriptBase<Spell> spellScript)
            return;

        if (spellScript.Subject.Template.Name.EqualsI("Zap"))
        {
            damage = Convert.ToInt32(damage + (attacker.StatSheet.EffectiveMaximumMp / 3));
        }
    }

    protected virtual int GetDefenderAc(Creature defender) => defender switch
    {
        Aisling aisling => Math.Clamp(
            aisling.UserStatSheet.EffectiveAc,
            WorldOptions.Instance.MinimumAislingAc,
            WorldOptions.Instance.MaximumAislingAc),
        _ => Math.Clamp(defender.StatSheet.EffectiveAc, WorldOptions.Instance.MinimumMonsterAc, WorldOptions.Instance.MaximumMonsterAc)
    };

    protected virtual void HandleClawFist(ref int damage, IScript source, Creature attacker)
    {
        if (!attacker.Effects.Contains("clawfist"))
            return;

        if (source is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (skillScript.Subject.Template.IsAssail)
            damage = Convert.ToInt32(damage * 1.3);
    }
}