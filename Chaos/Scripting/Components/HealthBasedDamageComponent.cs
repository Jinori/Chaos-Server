using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components;

public class HealthBasedDamageComponent : IComponent
{
    protected virtual int CalculateDamage(
        Creature source,
        decimal? healthMultiplier
    )
    {
        var multiplier = healthMultiplier ?? 1;
        var finalDamage = Convert.ToInt32(multiplier * source.StatSheet.CurrentHp);
        return finalDamage;
    }

    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IHealthBasedDamageComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var healthCostP = options.HealthCostPct ?? 1;

        foreach (var target in targets)
        {
            var damage = CalculateDamage(
                context.Source,
                options.HealthMultiplier);

            if (damage <= 0)
                continue;

            if (options.TrueDamage) 
                options.ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
            
            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                options.SourceScript,
                damage,
                options.Element);
        }
        
        if (context.Source.StatSheet.HealthPercent <= options.HealthCostPct)
            context.Source.StatSheet.SetHp(1);
        else
            context.Source.StatSheet.SubtractHealthPct(healthCostP);

        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    public interface IHealthBasedDamageComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        Element? Element { get; init; }
        decimal? HealthMultiplier { get; init; }
        int? HealthCostPct { get; init; }
        bool TrueDamage { get; init; }
        IScript SourceScript { get; init; }
    }
}