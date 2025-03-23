using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct CunningEffectAbilityComponent : IComponent
{

    private readonly IEffectFactory EffectFactory;

    public CunningEffectAbilityComponent(IEffectFactory effectFactory)
    {
        EffectFactory = effectFactory;
    }

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {

        var targets = vars.GetTargets<Creature>();
        var effect1 = EffectFactory.Create("Cunning1");
        var effect2 = EffectFactory.Create("Cunning2");
        var effect3 = EffectFactory.Create("Cunning3");
        var effect4 = EffectFactory.Create("Cunning4");
        var effect5 = EffectFactory.Create("Cunning5");
        var effect6 = EffectFactory.Create("Cunning6");
        
        
        foreach (var target in targets)
        {
            if (!target.Effects.Contains("Cunning1")
                || !target.Effects.Contains("Cunning2")
                || !target.Effects.Contains("Cunning3")
                || !target.Effects.Contains("Cunning4")
                || !target.Effects.Contains("Cunning5")
                || (!target.Effects.Contains("Cunning6") 
                    && (target.StatSheet.EffectiveMaximumMp >= 4000)))
            {
                target.Effects.Apply(context.Source, effect1);
                return;
            }
            if (target.Effects.Contains("Cunning1") && (target.StatSheet.EffectiveMaximumMp >= 8000))
            {
                target.Effects.Terminate("Cunning1");
                target.Effects.Apply(context.Source, effect2);
                return;
            }
            
            if (target.Effects.Contains("Cunning2") && (target.StatSheet.EffectiveMaximumMp >= 16000))
            {
                target.Effects.Terminate("Cunning2");
                target.Effects.Apply(context.Source, effect3);
                return;
            }
            if (target.Effects.Contains("Cunning3") && (target.StatSheet.EffectiveMaximumMp >= 32000))
            {
                target.Effects.Terminate("Cunning3");
                target.Effects.Apply(context.Source, effect4);
                return;
            }
            if (target.Effects.Contains("Cunning4") && (target.StatSheet.EffectiveMaximumMp >= 64000))
            {
                target.Effects.Terminate("Cunning4");
                target.Effects.Apply(context.Source, effect5);
                return;
            }
            if (target.Effects.Contains("Cunning5") && (target.StatSheet.EffectiveMaximumMp >= 128000))
            {
                target.Effects.Terminate("Cunning5");
                target.Effects.Apply(context.Source, effect6);
                return;
            }
            context.SourceAisling?.SendOrangeBarMessage("You do not have the mana to increase your Cunning.");
        }
    }

    public interface IApplyEffectComponentOptions
    {
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
        int? EffectApplyChance { get; init; }
    }
}