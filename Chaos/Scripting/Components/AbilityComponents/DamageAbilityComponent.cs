#region
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct DamageAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var sourceScript = vars.GetSourceScript();

        var surroundingTargets = options.SurroundingTargets ?? false;
        var numberOfTargets = surroundingTargets ? targets.Count() : 1;

        foreach (var target in targets)
        {
            var damage = CalculateDamage(
                context.Source,
                target,
                numberOfTargets,
                options.DamageMultiplierPerTarget ?? 0.05m, // Default to 5% if not specified
                options.BaseDamage,
                options.PctHpDamage,
                options.DamageStat,
                options.DamageStatMultiplier,
                options.MoreDmgLowTargetHp,
                options.PctOfMana,
                options.PctOfManaMultiplier,
                options.PctOfHealth,
                options.PctOfHealthMultiplier);

            if (!sourceScript.ScriptKey.Contains("AssassinStrike") && (damage > 1000000))
                damage = 1000000;
            
            if (damage > 0)
                options.ApplyDamageScript.ApplyDamage(
                    context.Source,
                    target,
                    sourceScript,
                    damage,
                    options.Element);
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
        bool? moreDmgLowTargetHp = null,
        decimal? pctOfMana = null,
        decimal? pctOfManaMultiplier = null,
        decimal? pctOfHealth = null,
        decimal? pctOfHealthMultiplier = null)
    {
        var finalDamage = baseDamage ?? 0;

        // Calculate damage based on the target's HP
        if (moreDmgLowTargetHp == true)
        {
            var healthPercentFactor = 1 + (1 - target.StatSheet.HealthPercent / 100m);

            finalDamage += Convert.ToInt32(
                MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0) * healthPercentFactor);
        }

        if (pctHpDamage.HasValue)
        {
            var pctDmg = DamageHelper.CalculatePercentDamage(source, target, pctHpDamage.Value);
            
            finalDamage += pctDmg;
        }

        if (pctOfHealth.HasValue)
        {
            var healthDamage = MathEx.GetPercentOf<int>((int)source.StatSheet.EffectiveMaximumHp, pctOfHealth.Value);

            if (pctOfHealthMultiplier.HasValue)
                healthDamage = Convert.ToInt32(healthDamage * pctOfHealthMultiplier.Value);

            finalDamage += healthDamage;
        }

        // Add damage based on a specific stat (like Int, Str, etc.)
        if (damageStat.HasValue)
            finalDamage += damageStatMultiplier.HasValue
                ? Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value)
                : source.StatSheet.GetEffectiveStat(damageStat.Value);

        // Apply mana-based damage
        if (pctOfMana.HasValue)
        {
            var manaDamage = MathEx.GetPercentOf<int>((int)source.StatSheet.EffectiveMaximumMp, pctOfMana.Value);

            if (pctOfManaMultiplier.HasValue)
                manaDamage = Convert.ToInt32(manaDamage * pctOfManaMultiplier.Value);

            finalDamage += manaDamage;
        }

        // Apply damage multiplier based on the number of surrounding targets
        if (numberOfTargets > 1)
            finalDamage = (int)(finalDamage * (1 + damageMultiplierPerTarget * (numberOfTargets - 1)));

        return finalDamage;
    }

    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        decimal? DamageMultiplierPerTarget { get; init; } // New option for damage multiplier per target
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
        bool? MoreDmgLowTargetHp { get; init; }
        decimal? PctHpDamage { get; init; }
        decimal? PctOfHealth { get; init; }
        decimal? PctOfHealthMultiplier { get; init; }

        // Mana-based damage fields
        decimal? PctOfMana { get; init; }
        decimal? PctOfManaMultiplier { get; init; }
        bool? SurroundingTargets { get; init; } // New option to check for surrounding targets
    }
}