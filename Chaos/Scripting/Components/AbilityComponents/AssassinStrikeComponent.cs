using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
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
                options.PctHpDamage,
                options.DamageStat,
                options.DamageStatMultiplier);

            if (target is Monster monster)
            {
                if (monster.Script.Is<ThisIsABossScript>())
                    continue;

                // 20% chance to kill the target instantly
                if (IntegerRandomizer.RollChance(20))
                {
                    damage = 999999999;
                }
            }

            if (damage > 0)
            {
                options.ApplyDamageScript.ApplyDamage(
                    context.Source,
                    target,
                    options.SourceScript,
                    damage,
                    options.Element);
            }
        }
    }

    private int CalculateDamage(
        Creature source,
        Creature target,
        int? baseDamage = null,
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null)
    {
        var finalDamage = baseDamage ?? 0;

        // Scale damage based on the target's current health.
        if (pctHpDamage.HasValue)
        {
            var healthDamage = target.StatSheet.CurrentHp * (pctHpDamage.Value / 100m);
            finalDamage += Convert.ToInt32(healthDamage);
        }

        // Add additional damage based on the source's stat.
        if (damageStat != null)
        {
            finalDamage += damageStatMultiplier.HasValue
                ? Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value)
                : source.StatSheet.GetEffectiveStat(damageStat.Value);
        }

        if (target is not Monster monster)
            return finalDamage;
        
        if (monster.AggroList.TryGetValue(source.Id, out _)) 
            return finalDamage;
            
        var bonusDamage = finalDamage * .50;
        finalDamage += Convert.ToInt32(bonusDamage);

        return finalDamage;
    }


    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
        decimal? PctHpDamage { get; init; }
        IScript SourceScript { get; init; }
    }
}
