using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct DamageAbilityComponent : IComponent
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
        else
        {
            finalDamage += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);
        }

        if (!damageStat.HasValue)
        {
            ApplyFuryEffects(source, ref finalDamage);
            return finalDamage;
        }

        finalDamage += damageStatMultiplier.HasValue
            ? Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value)
            : source.StatSheet.GetEffectiveStat(damageStat.Value);

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
    }
}
