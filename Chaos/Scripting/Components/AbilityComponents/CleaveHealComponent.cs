using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.Components.AbilityComponents;

public class CleaveHealComponent : IComponent
{
    private Animation Animate { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 97
    };

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ICleaveHealComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        int targetsHit = 0;
        
        foreach (var target in targets)
            if (target.IsAlive)
                targetsHit++;

        var missingHealth = context.Source.StatSheet.EffectiveMaximumHp - context.Source.StatSheet.CurrentHp;
        var healAmount = MathEx.GetPercentOf<int>((int)missingHealth, options.HealPercentMissingHealth) * targetsHit;

        if (healAmount >= 1)
        {
            options.ApplyHealScript.ApplyHeal(context.Source, context.Source, vars.GetSourceScript(), healAmount);
            context.SourceAisling?.SendActiveMessage($"You've been healed by {healAmount} from {targetsHit.ToWords()} targets!");
        }

        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
        context.SourceAisling?.Animate(Animate);
    }

    public interface ICleaveHealComponentOptions
    {
        IApplyDamageScript ApplyDamageScript { get; init; }
        IApplyHealScript ApplyHealScript { get; init; }
        decimal DmgHealthPct { get; init; }
        decimal HealPercentMissingHealth { get; init; }
    }
}