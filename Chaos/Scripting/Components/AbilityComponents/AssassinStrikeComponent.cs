using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.MonsterScripts;
using Chaos.Scripting.MonsterScripts.Boss;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct AssassinStrikeComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            var damage = CalculateDamage(
                context.Source,
                target,
                options.BaseDamage,
                options.DamageStat,
                options.DamageStatMultiplier,
                options.PctVitalityDmg);

            //add True Hide damage multiplier
            damage = (int)(damage * options.DmgMultiplier);
            
            //if the target is an aisling
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (target is Aisling aisling)
                //if we were the last person to hit them, do 1% of normal dmg
                if (aisling.Trackers.LastDamagedBy?.Equals(context.Source) == true)
                    damage = (int)(damage * .01m);
            
            //maybe make assassin strike deal 33% of vitality as dmg? plus some decent base dmg?
            // would def make rogues want balanced hp/mp

            // 10% chance to kill the target instantly (if not training dummy or boss)
            if (target is Monster && !target.Script.Is<TrainingDummyScript>() && !target.Script.Is<ThisIsABossScript>())
                if (IntegerRandomizer.RollChance(25))
                    damage = target.StatSheet.CurrentHp;

            if (damage > 0)
                options.ApplyDamageScript.ApplyDamage(
                    context.Source,
                    target,
                    vars.GetSourceScript(),
                    damage,
                    options.Element);
        }
    }

    private int CalculateDamage(
        Creature source,
        Creature target,
        int? baseDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null,
        decimal? pctVitalityDmg = null)
    {
        var finalDamage = baseDamage ?? 0;
        
        // Add additional damage based on the source's stat.
        if (damageStat != null)
            finalDamage += damageStatMultiplier.HasValue
                ? Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value)
                : source.StatSheet.GetEffectiveStat(damageStat.Value);

        //10x stat dmg vs monsters
        if (target is Monster)
            finalDamage *= 10;

        if (pctVitalityDmg.HasValue)
            finalDamage += MathEx.GetPercentOf<int>(source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2, pctVitalityDmg.Value); 
        
        if (target is not Monster monster)
            return finalDamage;

        // 1.25x damage vs monsters if you're not on the aggro list
        if (!monster.AggroList.TryGetValue(source.Id, out _))
            finalDamage = (int)(finalDamage * 1.25m);

        return finalDamage;
    }

    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        decimal DmgMultiplier { get; set; }
        Element? Element { get; init; }
        decimal? PctVitalityDmg { get; init; }
    }
}