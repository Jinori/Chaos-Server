using System.Collections.Immutable;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Servers.Options;

// ReSharper disable UseCollectionExpression

namespace Chaos.Formulae.Damage;

public class DefaultDamageFormula : IDamageFormula
{
    protected virtual ImmutableArray<ImmutableArray<decimal>> ElementalModifierLookup { get; } = new[]
    {
        // @formatter:off
        //mostly lifted from http://da-wizard.com/elements.html
        //                                                  D E F E N S E
        //                         None,  Fire,  Water,  Wind,  Earth, Holy,  Darkness,  Wood,  Metal,  Undead
        /*      None*/     new[] { 1.00m, 0.45m, 0.45m,  0.45m, 0.45m, 0.45m, 0.45m,     0.50m, 0.45m,  0.50m }.ToImmutableArray(),
        /* O    Fire*/     new[] { 2.00m, 0.50m, 1.00m,  1.50m, 1.00m, 0.75m, 0.75m,     1.25m, 1.25m,  1.00m }.ToImmutableArray(),
        /* F    Water*/    new[] { 2.00m, 1.50m, 0.50m,  1.00m, 1.00m, 0.75m, 0.75m,     1.25m, 1.25m,  1.00m }.ToImmutableArray(), 
        /* F    Wind*/     new[] { 2.00m, 1.00m, 1.00m,  0.50m, 1.50m, 0.75m, 0.75m,     0.75m, 0.75m,  1.00m }.ToImmutableArray(),
        /* E    Earth*/    new[] { 2.00m, 1.00m, 1.50m,  1.00m, 0.50m, 0.75m, 0.75m,     0.75m, 0.75m,  1.00m }.ToImmutableArray(),
        /* N    Holy*/     new[] { 2.00m, 1.00m, 1.00m,  1.00m, 1.00m, 0.50m, 1.50m,     1.20m, 1.20m,  1.00m }.ToImmutableArray(),
        /* S    Darkness*/ new[] { 2.00m, 1.00m, 1.00m,  1.00m, 1.00m, 1.50m, 0.50m,     1.20m, 1.20m,  1.00m }.ToImmutableArray(),
        /* E    Wood*/     new[] { 2.00m, 0.75m, 0.75m,  1.20m, 1.20m, 1.00m, 1.00m,     1.00m, 1.00m,  1.00m }.ToImmutableArray(),
        /*      Metal*/    new[] { 2.00m, 0.75m, 0.75m,  1.25m, 1.25m, 0.80m, 0.80m,     1.00m, 1.00m,  1.00m }.ToImmutableArray(),
        /*      Undead*/   new[] { 2.00m, 1.25m, 1.25m,  0.75m, 0.75m, 0.80m, 0.80m,     1.00m, 1.00m,  1.00m }.ToImmutableArray() 
        // @formatter:on
    }.ToImmutableArray();

    /// <inheritdoc />
    public int Calculate(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null)
    {
        if (target.IsGodModeEnabled() || target.Effects.Contains("Invulnerability"))
            return 0;

        ApplySkillSpellModifier(ref damage, script, source);

        var defenderAc = GetDefenderAc(target);

        ApplyAcModifier(ref damage, defenderAc);

        ApplyElementalModifier(
            ref damage,
            elementOverride ?? source.StatSheet.OffenseElement,
            target.StatSheet.DefenseElement,
            script);

        HandleClawFist(ref damage, script, source);
        HandleChaosFist(ref damage, script, source);
        HandleAite(ref damage, target);
        HandleWeaponDamage(ref damage, source);
        HandleDmgStat(ref damage, script, source);
        HandleEffectDmg(ref damage, target);

        return damage;
    }

    protected virtual void ApplyAcModifier(ref int damage, int defenderAc) => damage = Convert.ToInt32(damage * (1 + defenderAc / 100m));

    protected virtual void ApplyElementalModifier(
        ref int damage,
        Element attackElement,
        Element defenseElement,
        IScript script)
        => damage = Convert.ToInt32(damage * ElementalModifierLookup[(int)attackElement][(int)defenseElement]);

    protected virtual void ApplySkillSpellModifier(ref int damage, IScript source, Creature attacker)
    {
        switch (source)
        {
            case ISkillScript:
            {
                if (source is not SubjectiveScriptBase<Skill> skillScript)
                    return;

                var addedFromPct = damage * (attacker.StatSheet.EffectiveSkillDamagePct / 100m);

                if (skillScript.Subject.Template.IsAssail)
                    addedFromPct = 0;
                damage += Convert.ToInt32(attacker.StatSheet.EffectiveFlatSkillDamage + addedFromPct);

                break;
            }
            case ISpellScript:
            {
                var addedFromPct = damage * (attacker.StatSheet.EffectiveSpellDamagePct / 100m);
                damage += Convert.ToInt32(attacker.StatSheet.EffectiveFlatSpellDamage + addedFromPct);

                break;
            }
            case IReactorTileScript reactorTileScript:
            {
                if (reactorTileScript.Is<SubjectiveScriptBase<ReactorTile>>(out var subjectReactorTileScript))
                {
                    var sourceScript = subjectReactorTileScript.Subject.SourceScript;

                    // ReSharper disable once TailRecursiveCall
                    if (sourceScript is not null && sourceScript is not IReactorTileScript)
                        ApplySkillSpellModifier(ref damage, sourceScript, attacker);
                }

                break;
            }
        }
    }

    protected virtual int GetDefenderAc(Creature defender)
        => defender switch
        {
            Aisling aisling => Math.Clamp(
                aisling.UserStatSheet.EffectiveAc,
                WorldOptions.Instance.MinimumAislingAc,
                WorldOptions.Instance.MaximumAislingAc),
            _ => Math.Clamp(defender.StatSheet.EffectiveAc, WorldOptions.Instance.MinimumMonsterAc, WorldOptions.Instance.MaximumMonsterAc)
        };

    protected virtual void HandleAite(ref int damage, Creature defender)
    {
        if (defender.IsBeagAited())
            damage = (int)(damage * 0.92);

        if (defender.IsAited())
            damage = (int)(damage * 0.85);

        if (defender.IsMorAited())
            damage = (int)(damage * 0.78);

        if (defender.IsArdAited())
            damage = (int)(damage * 0.70);

        if (defender.IsBlessed())
            damage = (int)(damage * 0.65);

        if (defender.IsTormented())
            damage = (int)(damage * 0.68);
    }

    protected virtual void HandleChaosFist(ref int damage, IScript source, Creature attacker)
    {
        if (!attacker.Effects.Contains("Chaos Fist"))
            return;

        if (source is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (skillScript.Subject.Template.IsAssail)
            damage = Convert.ToInt32(damage * 1.6);
    }

    protected virtual void HandleClawFist(ref int damage, IScript source, Creature attacker)
    {
        if (!attacker.Effects.Contains("Claw Fist"))
            return;

        if (source is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (skillScript.Subject.Template.IsAssail)
            damage = Convert.ToInt32(damage * 1.2);
    }

    protected virtual void HandleDmgStat(ref int damage, IScript source, Creature attacker)
    {
        if (source is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (!skillScript.Subject.Template.IsAssail)
            return;

        var damageMultiplier = 1 + attacker.StatSheet.EffectiveDmg * 0.01;
        damage = (int)(damage * damageMultiplier);
    }

    protected virtual void HandleEffectDmg(ref int damage, Creature defender)
    {
        var damageMultiplier = 1.0;

        if (defender.Effects.Contains("Beag Pramh"))
            damageMultiplier *= 1.25;

        if (defender.Effects.Contains("Wolf Fang Fist") || defender.Effects.Contains("Crit") || defender.Effects.Contains("Pramh"))
            damageMultiplier *= 2.0;
        var modifiedDamage = damage * damageMultiplier;

        // Round the modified damage to the nearest whole number.
        damage = (int)Math.Round(modifiedDamage);
    }

    protected virtual void HandleWeaponDamage(ref int damage, Creature attacker)
    {
        if (attacker is not Aisling aisling || !aisling.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var obj))
            return;

        damage += obj.Modifiers.FlatSkillDamage;
    }
}