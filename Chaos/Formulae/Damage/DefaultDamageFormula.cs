using System.Collections.Immutable;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.EffectScripts.Warrior;
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

        if (target.Name == "Shamensyth")
            if ((elementOverride ?? source.StatSheet.OffenseElement) == Element.Fire)
            {
                if (source is Aisling aisling)
                    aisling.SendOrangeBarMessage("Shamensyth is immune to the fire!");

                return 0;
            }

        ApplySourceModifiers(ref damage, script, source);
        
        ApplyElementalModifier(
            ref damage,
            elementOverride ?? source.StatSheet.OffenseElement,
            target.StatSheet.DefenseElement,
            script);
        
        ApplyTargetModifiers(ref damage, source, target, script);

        return damage;
    }

    protected virtual void ApplyAcModifier(ref int damage, int defenderAc) => damage = Convert.ToInt32(damage * (1 + defenderAc / 100m));

    protected virtual void ApplyElementalModifier(
        ref int damage,
        Element attackElement,
        Element defenseElement,
        IScript script)
        => damage = Convert.ToInt32(damage * ElementalModifierLookup[(int)attackElement][(int)defenseElement]);

    protected virtual void ApplySourceModifiers(ref int damage, IScript source, Creature attacker)
    {
        switch (source)
        {
            case ISkillScript or WrathEffect or WhirlwindEffect or InfernoEffect:
            {
                var addedFromPct = damage * (attacker.StatSheet.EffectiveSkillDamagePct / 100m);

                if (IsAssail(source))
                {
                    addedFromPct = 0;
                    HandleDmgStat(ref damage, source, attacker);
                }
                
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
                        ApplySourceModifiers(ref damage, sourceScript, attacker);
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

    protected virtual bool IsAssail(IScript source)
    {
        if (source is SubjectiveScriptBase<Skill> { Subject.Template.IsAssail: true } or WrathEffect or WhirlwindEffect or InfernoEffect)
            return true;

        return false;
    }

    protected virtual void HandleDmgStat(ref int damage, IScript source, Creature attacker)
    {
        var damageMultiplier = 1 + attacker.StatSheet.EffectiveDmg * 0.01;
        damage = (int)(damage * damageMultiplier);
    }

    protected virtual void ApplyTargetModifiers(
        ref int damage,
        Creature attacker,
        Creature defender,
        IScript script)
    {
        //defensive things are multiplicative
        var defenderAc = GetDefenderAc(defender);

        ApplyAcModifier(ref damage, defenderAc);
        HandleAite(ref damage, defender);

        //offensive things are additive
        var damageMultiplier = 1.0;

        if (defender.Effects.Contains("Beag Pramh"))
            damageMultiplier += .25;
        else if (defender.Effects.Contains("Wolf Fang Fist") || defender.Effects.Contains("Pramh") || (defender.Effects.TryGetEffect("FourfoldSeal", out var seal) && (seal.Source.Equals(attacker))))
            damageMultiplier += 1;

        if (defender.Effects.Contains("Crit"))
            if (script is SubjectiveScriptBase<Skill> { Subject.Template.IsAssail: false } or SubjectiveScriptBase<Spell>)
            {
                damageMultiplier += 0.3;
                defender.Effects.Dispel("Crit");
            }

        var relation = attacker.DirectionalRelationTo(defender);
        var reverse = defender.Direction.Reverse();
        var isBehind = relation == reverse;
        var isFlank = (relation != defender.Direction) && !isBehind;

        if (defender.Effects.Contains("dodge"))
        {
            if (isBehind)
                damageMultiplier += 0.4;
            else if (isFlank)
                damageMultiplier += 0.2;
        } else if (defender.Effects.Contains("evasion"))
        {
            if (isBehind)
                damageMultiplier += 0.3;
            else if (isFlank)
                damageMultiplier += 0.1;
        } else
        {
            if (isBehind)
                damageMultiplier += 0.5;
            else if (isFlank)
                damageMultiplier += 0.25;
        }

        if (attacker is Aisling { UserStatSheet.BaseClass: BaseClass.Rogue })
            if (isBehind)
                damageMultiplier += 0.25;
            else if (isFlank)
                damageMultiplier += 0.25 / 2.0;

        var modifiedDamage = damage * damageMultiplier;

        damage = (int)modifiedDamage;
    }
}