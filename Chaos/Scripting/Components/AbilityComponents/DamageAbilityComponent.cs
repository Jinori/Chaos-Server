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

public struct DamageAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        bool surroundingTargets = options.SurroundingTargets ?? false;
        int numberOfTargets = surroundingTargets ? targets.Count() : 1;
        
        foreach (var target in targets)
        {
            var damage = CalculateDamage(
                context.Source,
                target,
                numberOfTargets,
                options.DamageMultiplierPerTarget ?? 0.05m,  // Default to 5% if not specified
                options.BaseDamage,
                options.PctHpDamage,
                options.DamageStat,
                options.DamageStatMultiplier,
                options.MoreDmgLowTargetHp);

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
        int numberOfTargets,
        decimal damageMultiplierPerTarget,
        int? baseDamage = null,
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null,
        bool? moreDmgLowTargetHp = null)
    {
        var finalDamage = baseDamage ?? 0;
        
        if (moreDmgLowTargetHp == true)
        {
            var healthPercentFactor = 1 + (1 - target.StatSheet.HealthPercent / 100m);
            finalDamage += Convert.ToInt32(
                MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0) * healthPercentFactor);
        }

        if (pctHpDamage.HasValue)
        {
            if (!target.Script.Is<ThisIsABossScript>())
            {
                finalDamage += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);
            }    
            if (target.Script.Is<ThisIsABossScript>())
            {
                if (pctHpDamage > 2)
                {
                    pctHpDamage = 2;
                }
                finalDamage += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, (decimal)pctHpDamage!);
            }
        }

        if (!damageStat.HasValue)
        {
            ApplyFuryEffects(source, ref finalDamage);
            return finalDamage;
        }

        finalDamage += damageStatMultiplier.HasValue
            ? Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value)
            : source.StatSheet.GetEffectiveStat(damageStat.Value);

        // Apply the damage multiplier based on the number of surrounding targets
        if (numberOfTargets > 1)
        {
            finalDamage = (int)(finalDamage * (1 + damageMultiplierPerTarget * (numberOfTargets - 1)));
        }

        ApplyFuryEffects(source, ref finalDamage);
        
        return finalDamage;
    }

    private void ApplyFuryEffects(Creature source, ref int finalDamage)
    {
        var furyMultipliers = new Dictionary<string, double>
        {
            { "fury1", 1.15 },
            { "fury2", 1.30 },
            { "fury3", 1.50 },
            { "fury4", 1.70 },
            { "fury5", 1.95 },
            { "fury6", 2.20 }
        };

        foreach (var fury in furyMultipliers)
        {
            if (source.Effects.Contains(fury.Key))
            {
                finalDamage = (int)(finalDamage * fury.Value);
            }
        }
    }

    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
        bool? MoreDmgLowTargetHp { get; init; }
        decimal? PctHpDamage { get; init; }
        IScript SourceScript { get; init; }
        bool? SurroundingTargets { get; init; }  // New option to check for surrounding targets
        decimal? DamageMultiplierPerTarget { get; init; }  // New option for damage multiplier per target
    }
}
