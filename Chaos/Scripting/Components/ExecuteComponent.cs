using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
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
        var hasKilled = false;

        foreach (var target in targets)
        {
            if (target.StatSheet.HealthPercent <= options.KillTargetAtHealthPct)
            {
                options.ApplyDamageScript.ApplyDamage(context.Source, target, options.SourceScript, 9999);
                
                var healAmount = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, options.HealAmountIfExecuted);
                options.ApplyHealScript.ApplyHeal(target, context.Source, options.SourceScript, healAmount);
                context.SourceAisling?.SendActiveMessage($"You've been healed by {healAmount} from Execute!");

                if (!target.IsAlive)
                    hasKilled = true;
            } 
            else
            {
                var tenPercent = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, options.DmgHealthPct);
                options.ApplyDamageScript.ApplyDamage(context.Source, target, options.SourceScript, tenPercent);
            }
        }

        if (hasKilled)
        {
            options.PanelEntityBase.Elapsed = options.PanelEntityBase.Cooldown / 2;
        }
    }
    
    public interface IExecuteComponentOptions
    {
        PanelEntityBase PanelEntityBase { get; init; }
        IApplyHealScript ApplyHealScript { get; init; }
        IApplyDamageScript ApplyDamageScript { get; init; }
        IScript SourceScript { get; init; }
        int? KillTargetAtHealthPct { get; init; }
        int? CooldownReduction { get; init; }
        decimal HealAmountIfExecuted { get; init; }
        decimal DmgHealthPct { get; init; }
    }
}