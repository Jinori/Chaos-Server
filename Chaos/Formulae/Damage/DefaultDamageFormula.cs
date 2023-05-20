using System.Collections.Immutable;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Formulae.Damage;

public class DefaultDamageFormula : IDamageFormula
{
    protected virtual ImmutableArray<ImmutableArray<decimal>> ElementalModifierLookup { get; } = new[]
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
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null
    )
    {
        ApplySkillSpellModifier(ref damage, script, source);

        var defenderAc = GetDefenderAc(target);

        ApplyAcModifier(ref damage, defenderAc);
        ApplyElementalModifier(ref damage, elementOverride ?? source.StatSheet.OffenseElement, target.StatSheet.DefenseElement);
        HandleClawFist(ref damage, script, source);
        HandleAite(ref damage, target);
        HandleWeaponDamage(ref damage, source);
        return damage;
    }

    protected virtual void HandleAite(ref int damage, Creature defender)
    {
        if (!defender.Effects.Any(x => x.Name.EndsWithI("aite")))
            return;

        if (defender.Status.HasFlag(Status.BeagAite))
        {
            damage *= Convert.ToInt32(0.92);
        }

        if (defender.Status.HasFlag(Status.Aite))
        {
            damage *= Convert.ToInt32(0.85);
        }

        if (defender.Status.HasFlag(Status.MorAite))
        {
            damage *= Convert.ToInt32(0.78);
        }

        if (defender.Status.HasFlag(Status.ArdAite))
        {
            damage *= Convert.ToInt32(0.70);
        }
    }

    protected virtual void HandleWeaponDamage(ref int damage, Creature attacker)
    {
        if (attacker is not Aisling aisling || !aisling.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var obj)) 
            return;
        
        if (obj.Modifiers != null) 
            damage += obj.Modifiers.Dmg;
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