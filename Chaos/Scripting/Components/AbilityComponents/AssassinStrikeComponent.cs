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
                options.PctHpDamage,
                options.DamageStat,
                options.DamageStatMultiplier);

            if (target is Monster monster)

                // 10% chance to kill the target instantly
                if (IntegerRandomizer.RollChance(10))
                    if (!target.Script.Is<TrainingDummyScript>() && !target.Script.Is<ThisIsABossScript>())
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
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null)
    {
        var finalDamage = baseDamage ?? 0;

        // Scale damage based on the target's current health.
        if (pctHpDamage.HasValue)
        {
            var healthDamage = target.StatSheet.CurrentHp * (pctHpDamage.Value / 100m);

            if (healthDamage > 200000)
                healthDamage = 200000;

            finalDamage += Convert.ToInt32(healthDamage);
        }

        // Add additional damage based on the source's stat.
        if (damageStat != null)
            finalDamage += damageStatMultiplier.HasValue
                ? Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value)
                : source.StatSheet.GetEffectiveStat(damageStat.Value);

        if (target is not Monster monster)
            return finalDamage;
        
        var multiplier = 1.0m;

        if (!monster.AggroList.TryGetValue(source.Id, out _))
            multiplier += 0.25m;
        
        if(source.Effects.TryGetEffect("True Hide", out _))
            multiplier += 0.25m;

        var bonusDamage = finalDamage * multiplier;
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
    }
}