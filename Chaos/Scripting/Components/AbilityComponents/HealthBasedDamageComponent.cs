using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public class HealthBasedDamageComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IHealthBasedDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var healthCostP = options.HealthCostPct ?? 1;

        foreach (var target in targets)
        {
            var damage = CalculateDamage(context.Source, options.HealthMultiplier);

            if (damage <= 0)
                continue;

            if (options.TrueDamage)
                options.ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;

            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                vars.GetSourceScript(),
                damage,
                options.Element);
        }

        if (context.Source.StatSheet.HealthPercent <= options.HealthCostPct)
            context.Source.StatSheet.SetHp(1);
        else
            context.Source.StatSheet.SubtractHealthPct(healthCostP);

        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    protected virtual int CalculateDamage(Creature source, decimal? healthMultiplier)
    {
        var multiplier = healthMultiplier ?? 0.3m;
        var finalDamage = Convert.ToInt32(multiplier * source.StatSheet.CurrentHp);

        return finalDamage;
    }

    public interface IHealthBasedDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        Element? Element { get; init; }
        int? HealthCostPct { get; init; }
        decimal? HealthMultiplier { get; init; }
        bool TrueDamage { get; init; }
    }
}