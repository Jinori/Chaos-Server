using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components;

public class ExecuteComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IExecuteComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            if (target.StatSheet.HealthPercent <= options.KillTargetAtHealthPct)
            {
                target.StatSheet.SetHp(0);
                options.ApplyDamageScript.ApplyDamage(context.Source, target, options.SourceScript, 9999);
                
                var healAmount = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, options.HealAmountIfExecuted);
                options.ApplyHealScript.ApplyHeal(target, context.Source, options.SourceScript, healAmount);
                
                //Needs Cooldown Reduction
            } 
            else
            {
                var tenPercent = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, options.DmgHealthPct);
                options.ApplyDamageScript.ApplyDamage(context.Source, target, options.SourceScript, tenPercent);
            }
        }
    }
    
    public interface IExecuteComponentOptions
    {
        IApplyHealScript ApplyHealScript { get; init; }
        IApplyDamageScript ApplyDamageScript { get; init; }
        IScript SourceScript { get; init; }
        int? KillTargetAtHealthPct { get; init; }
        int? CooldownReduction { get; init; }
        decimal HealAmountIfExecuted { get; init; }
        decimal DmgHealthPct { get; init; }
    }
}