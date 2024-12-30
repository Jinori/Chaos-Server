using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public class CritDamageComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            var damage = CalculateDamage(
                context.Source,
                target,
                options.BaseDamage,
                options.CritChance,
                options.PctHpDamage,
                options.DamageStat,
                options.DamageStatMultiplier,
                options.MoreDmgLowTargetHp);

            if (damage <= 0)
                continue;

            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                vars.GetSourceScript(),
                damage,
                options.Element);
        }
    }

    protected virtual int CalculateDamage(
        Creature source,
        Creature target,
        int? baseDamage = null,
        int? critChance = null,
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null,
        bool? moreDmgLowTargetHp = null
    )
    {
        var finalDamage = baseDamage ?? 0;

        if (moreDmgLowTargetHp is true)
        {
            var healthPercentFactor = 1 + (1 - target.StatSheet.HealthPercent / 100m);

            finalDamage += Convert.ToInt32(
                MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0) * healthPercentFactor);
        }
        else
            finalDamage += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);

        if (!damageStat.HasValue)
            return finalDamage;

        if (!damageStatMultiplier.HasValue)
        {
            finalDamage += source.StatSheet.GetEffectiveStat(damageStat.Value);
            return finalDamage;
        }
        
        finalDamage += Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value);

        if (critChance.HasValue && !IntegerRandomizer.RollChance(critChance.Value))
            return finalDamage;
        
        var finalDamage1 = finalDamage * 2;
            
        return finalDamage1;
    }

    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        
        int? CritChance { get; init; }
        Element? Element { get; init; }
        bool? MoreDmgLowTargetHp { get; init; }
        decimal? PctHpDamage { get; init; }
    }
}