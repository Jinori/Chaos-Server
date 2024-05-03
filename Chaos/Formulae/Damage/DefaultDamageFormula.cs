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
        /*      None*/     new[] { 0.50m, 0.45m, 0.45m,  0.45m, 0.45m, 0.45m, 0.45m,     0.45m, 0.45m,  0.45m }.ToImmutableArray(),
        /* O    Fire*/     new[] { 2.00m, 0.50m, 1.00m,  2.00m, 1.00m, 1.00m, 1.00m,     2.01m, 0.83m,  2.01m }.ToImmutableArray(),
        /* F    Water*/    new[] { 2.00m, 2.00m, 0.50m,  1.00m, 1.00m, 0.93m, 0.83m,     0.58m, 1.74m,  0.93m }.ToImmutableArray(), 
        /* F    Wind*/     new[] { 2.00m, 1.00m, 1.00m,  0.50m, 2.00m, 0.93m, 0.83m,     1.03m, 1.74m,  0.83m }.ToImmutableArray(),
        /* E    Earth*/    new[] { 2.00m, 1.00m, 2.00m,  1.00m, 0.50m, 0.93m, 0.83m,     0.58m, 0.83m,  0.58m }.ToImmutableArray(),
        /* N    Holy*/     new[] { 2.00m, 0.76m, 0.76m,  0.76m, 0.76m, 0.50m, 1.48m,     0.58m, 0.76m,  2.01m }.ToImmutableArray(),
        /* S    Darkness*/ new[] { 2.00m, 1.25m, 1.25m,  1.25m, 1.25m, 1.48m, 0.50m,     1.48m, 0.58m,  0.58m }.ToImmutableArray(),
        /* E    Wood*/     new[] { 2.00m, 0.58m, 0.76m,  1.03m, 1.74m, 0.93m, 0.83m,     0.50m, 1.03m,  0.83m }.ToImmutableArray(),
        /*      Metal*/    new[] { 2.00m, 0.83m, 0.50m,  1.88m, 0.83m, 0.93m, 0.83m,     1.25m, 0.50m,  1.03m }.ToImmutableArray(),
        /*      Undead*/   new[] { 2.00m, 0.50m, 0.83m,  0.83m, 1.88m, 0.93m, 0.83m,     0.58m, 0.76m,  0.50m }.ToImmutableArray() 
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
        // Check if the source (attacker) has godmode enabled.
        var isGodModeEnabled = target is Aisling aisling && aisling.Trackers.Enums.HasValue(GodMode.Yes);

        // If godmode is enabled, set the damage to 0.
        if (isGodModeEnabled)
        {
            damage = 0;
            return damage;
        }
        
        ApplySkillSpellModifier(ref damage, script, source);

        var defenderAc = GetDefenderAc(target);

        ApplyAcModifier(ref damage, defenderAc);

        ApplyElementalModifier(
            ref damage,
            elementOverride ?? source.StatSheet.OffenseElement,
            target.StatSheet.DefenseElement,
            script);

        HandleClawFist(ref damage, script, source);
        HandleAite(ref damage, target);
        HandleWeaponDamage(ref damage, source);
        HandleDmgStat(ref damage, script, source);
        HandleEffectDmg(ref damage, target);
        return damage;
    }

    protected virtual void ApplyAcModifier(ref int damage, int defenderAc) =>
        damage = Convert.ToInt32(damage * (1 + defenderAc / 100m));

    protected virtual void ApplyElementalModifier(ref int damage, Element attackElement, Element defenseElement, IScript script)
    {
        if (script is SubjectiveScriptBase<Spell> { Subject.Template.TemplateKey: "zap" })
            return;

        damage = Convert.ToInt32(damage * ElementalModifierLookup[(int)attackElement][(int)defenseElement]);
        
    }

    protected virtual void ApplySkillSpellModifier(ref int damage, IScript source, Creature attacker)
    {
        switch (source)
        {
            case ISkillScript:
            {
                var addedFromPct = damage * (attacker.StatSheet.EffectiveSkillDamagePct / 100m);
                damage += Convert.ToInt32(attacker.StatSheet.EffectiveFlatSkillDamage + addedFromPct);

                break;
            }
            case ISpellScript:
            {
                var addedFromPct = damage * (attacker.StatSheet.EffectiveSpellDamagePct / 100m);
                damage += Convert.ToInt32(attacker.StatSheet.EffectiveFlatSpellDamage + addedFromPct);

                break;
            }
        }
    }

    protected virtual void HandleAite(ref int damage, Creature defender)
    {
        if (!defender.Effects.Any(x => x.Name.EndsWithI("aite")))
            return;

        if (defender.Status.HasFlag(Status.BeagAite))
        {
            damage = (int)(damage * 0.92);
        }

        if (defender.Status.HasFlag(Status.Aite))
        {
            damage = (int)(damage * 0.85);
        }

        if (defender.Status.HasFlag(Status.MorAite))
        {
            damage = (int)(damage * 0.78);
        }

        if (defender.Status.HasFlag(Status.ArdAite))
        {
            damage = (int)(damage * 0.70);
        }
    }


    protected virtual void HandleWeaponDamage(ref int damage, Creature attacker)
    {
        if (attacker is not Aisling aisling || !aisling.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var obj))
            return;

        damage += obj.Modifiers.Dmg;
    }

    protected virtual int GetDefenderAc(Creature defender)
        => defender switch
        {
            Aisling aisling => Math.Clamp(
                aisling.UserStatSheet.EffectiveAc,
                WorldOptions.Instance.MinimumAislingAc,
                WorldOptions.Instance.MaximumAislingAc),
            _ => Math.Clamp(
            defender.StatSheet.EffectiveAc,
            WorldOptions.Instance.MinimumMonsterAc,
            WorldOptions.Instance.MaximumMonsterAc)
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
    
    protected virtual void HandleDmgStat(ref int damage, IScript source, Creature attacker)
    {
        if (source is not SubjectiveScriptBase<Skill> skillScript)
            return;

        if (!skillScript.Subject.Template.IsAssail)
            return;

        var damageMultiplier = 1 + (attacker.StatSheet.EffectiveDmg * 0.01);
        damage = (int)(damage * damageMultiplier);
    }

    protected virtual void HandleEffectDmg(ref int damage, Creature defender)
    {
        var damageMultiplier = 1.0;
        
        if (defender.Effects.Contains("pramh"))
        {
            damageMultiplier *= 2.0;
        }

        if (defender.Effects.Contains("beagpramh"))
        {
            damageMultiplier *= 1.25;
        }

        if (defender.Effects.Contains("wolfFangFist"))
        {
            damageMultiplier *= 2.0;
        }
        var modifiedDamage = damage * damageMultiplier;

        // Round the modified damage to the nearest whole number.
        damage = (int)Math.Round(modifiedDamage);
    }
}