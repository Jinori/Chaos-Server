using System.Reactive.Subjects;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;

namespace Chaos.Scripting.Components.AbilityComponents;

public class ExecuteComponent : IComponent
{
    
    private Animation Animate { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 97
    };
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IExecuteComponentOptions>();
        var subject = vars.GetSubject<PanelEntityBase>();
        var script = (vars.GetSourceScript() as SubjectiveScriptBase<PanelEntityBase>)!;
        var targets = vars.GetTargets<Creature>();
        var hasKilled = false;

        foreach (var target in targets)
            if (!target.Script.Is<ThisIsABossScript>() && target.StatSheet.HealthPercent <= options.KillTargetAtHealthPct)
            {
                options.ApplyDamageScript.ApplyDamage(
                    context.Source,
                    target,
                    vars.GetSourceScript(),
                    99999999);

                var healAmount = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, options.HealAmountIfExecuted);

                if (options.HealAmountIfExecuted > 0)
                {
                    options.ApplyHealScript.ApplyHeal(
                        target,
                        context.Source,
                        vars.GetSourceScript(),
                        healAmount);
                
                    context.SourceAisling?.SendActiveMessage($"You've been healed by {healAmount}!"); 
                }
                
                context.SourceAisling?.Animate(Animate);

                if (!target.IsAlive)
                    hasKilled = true;
            }
            else
            {
                if (!target.Script.Is<ThisIsABossScript>())
                { 
                    var tenPercent = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, options.DmgHealthPct);
                    options.ApplyDamageScript.ApplyDamage(
                        context.Source,
                        target,
                        vars.GetSourceScript(),
                        tenPercent);
                }
                else
                {
                    var onepercent = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp,
                        options.DmgHealthPct);
                    options.ApplyDamageScript.ApplyDamage(
                        context.Source,
                        target,
                        vars.GetSourceScript(),
                        onepercent);
                }
            }

        if (hasKilled)
            subject.SetTemporaryCooldown(subject.Cooldown!.Value / 2); 
    }

    public interface IExecuteComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        IApplyHealScript ApplyHealScript { get; init; }
        decimal DmgHealthPct { get; init; }
        decimal HealAmountIfExecuted { get; init; }
        int? KillTargetAtHealthPct { get; init; }
    }
}