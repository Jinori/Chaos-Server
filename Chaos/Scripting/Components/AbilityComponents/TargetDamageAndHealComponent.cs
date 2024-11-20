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

public struct TargetDamageAndHealComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        var surroundingTargets = options.SurroundingTargets ?? false;
        var numberOfTargets = surroundingTargets ? targets.Count : 1;

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

        // Apply healing based on the number of surrounding targets
        if (options.HealUser ?? false)
        {
            ApplyHealing(context.Source, numberOfTargets,
                options.HealMultiplierPerTarget ?? 0.05m); // Default to 5% if not specified
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
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
                MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0) *
                healthPercentFactor);
        }
        else
        {
            finalDamage += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);
        }

        if (!damageStat.HasValue)
        {
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

        return finalDamage;
    }

    private void ApplyHealing(Creature source, int numberOfTargets, decimal healMultiplierPerTarget)
    {
        var healAmount = (int)(source.StatSheet.MaximumHp * healMultiplierPerTarget * numberOfTargets);
        source.StatSheet.AddHp(healAmount);
        source.ShowHealth();
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
        bool? SurroundingTargets { get; init; }
        decimal? DamageMultiplierPerTarget { get; init; }
        bool? HealUser { get; init; }  // New option to toggle healing
        decimal? HealMultiplierPerTarget { get; init; }  // New option for heal multiplier per target
    }
