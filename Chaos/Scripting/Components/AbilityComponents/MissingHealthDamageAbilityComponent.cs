using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct MissingHealthDamageAbilityComponent : IComponent
{
    /// <inheritdoc />
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
                options.Element,
                vars.GetSourceScript());

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
        Element? element = null,
        IScript sourceScript = null!)
    {
        // Calculate damage based on the source's missing health
        var missingHealth = source.StatSheet.EffectiveMaximumHp - source.StatSheet.CurrentHp;
        var damage = Convert.ToInt32(missingHealth * 0.6m); // Full missing health as damage

        // If there's a base damage, add it
        if (baseDamage.HasValue)
            damage += baseDamage.Value;

        // If there's a stat multiplier, apply it
        if (damageStat.HasValue && damageStatMultiplier.HasValue)
            damage += Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value);
        else if (damageStat.HasValue)
            damage += source.StatSheet.GetEffectiveStat(damageStat.Value);

        // Apply additional effects if any
        ApplyFuryEffects(source, ref damage);

        return damage;
    }

    private void ApplyFuryEffects(Creature source, ref int finalDamage)
    {
        var furyMultipliers = new Dictionary<string, double>
        {
            {
                "Fury1", 1.15
            },
            {
                "Fury2", 1.30
            },
            {
                "Fury3", 1.50
            },
            {
                "Fury4", 1.70
            },
            {
                "Fury5", 1.95
            },
            {
                "Fury6", 2.20
            }
        };

        foreach (var fury in furyMultipliers)
            if (source.Effects.Contains(fury.Key))
                finalDamage = (int)(finalDamage * fury.Value);
    }

    public interface IDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
    }
}