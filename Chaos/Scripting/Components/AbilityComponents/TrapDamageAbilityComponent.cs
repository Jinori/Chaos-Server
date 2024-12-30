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

public struct TrapDamageAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ITrapDamageComponentOptions>();
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

    private int CalculateDamage(
        Creature source,
        Creature target,
        int? baseDamage = null,
        decimal? pctHpDamage = null,
        Stat? damageStat = null,
        decimal? damageStatMultiplier = null,
        bool? moreDmgLowTargetHp = null
    )
    {
        var finalDamage = baseDamage ?? 0;
        var maxDamageFromPctHp = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpDamage ?? 0);

        if (moreDmgLowTargetHp is true)
        {
            var healthPercentFactor = 1 + (1 - target.StatSheet.HealthPercent / 100m);
            maxDamageFromPctHp = Convert.ToInt32(maxDamageFromPctHp * healthPercentFactor);
        }

        var damageCap = ((baseDamage ?? 0) + Convert.ToInt32((source.StatSheet.GetEffectiveStat(damageStat ?? default(Stat))) * (damageStatMultiplier ?? 0))) * 10;
        finalDamage += Math.Min(maxDamageFromPctHp, damageCap);

        if (!damageStat.HasValue)
            return finalDamage;

        if (!damageStatMultiplier.HasValue)
        {
            finalDamage += source.StatSheet.GetEffectiveStat(damageStat.Value);
            return finalDamage;
        }

        finalDamage += Convert.ToInt32(source.StatSheet.GetEffectiveStat(damageStat.Value) * damageStatMultiplier.Value);

        return finalDamage;
    }

    public interface ITrapDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        int? BaseDamage { get; init; }
        Stat? DamageStat { get; init; }
        decimal? DamageStatMultiplier { get; init; }
        Element? Element { get; init; }
        bool? MoreDmgLowTargetHp { get; init; }
        decimal? PctHpDamage { get; init; }
    }
}
