using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components;

public class RemoveAndApplyEffectComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IRemoveAndApplyEffectComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        if (string.IsNullOrEmpty(options.EffectKeyToRemove))
            return;

        if (options.RemoveAllEffects.HasValue)
            foreach (var target in targets)
            {
                foreach (var effect in target.Effects)
                    target.Effects.Dispel(effect.Name);
            }

        if (string.IsNullOrEmpty(options.EffectKeyToAddAfterRemoval))
            return;

        foreach (var target in targets)
        {
            if (options.EffectKeyToRemove is "beag cradh")
            {
                if (target.Effects.Contains("beag cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh")
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect);
                    target.Effects.Dispel(options.EffectKeyToRemove);
                } 
                else
                    return;
            }

            if (options.EffectKeyToRemove is "cradh")
            {
                if (target.Effects.Contains("cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh")
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect);
                    target.Effects.Dispel(options.EffectKeyToRemove);
                } 
                else
                    return;
            }

            if (options.EffectKeyToRemove is "mor cradh")
            {
                if (target.Effects.Contains("mor cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh")
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect);
                    target.Effects.Dispel(options.EffectKeyToRemove);
                } 
                else
                    return;
            }

            if (options.EffectKeyToRemove is "ard cradh")
            {
                if (target.Effects.Contains("ard cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh")
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect);
                    target.Effects.Dispel(options.EffectKeyToRemove);
                } 
                else
                    return;
            } 
            else
            {
                target.Effects.Dispel(options.EffectKeyToRemove);
                var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                target.Effects.Apply(context.Source, effect);
            }
        }
    }

    public interface IRemoveAndApplyEffectComponentOptions
    {
        IEffectFactory EffectFactory { get; init; }
        string? EffectKeyToAddAfterRemoval { get; init; }
        string? EffectKeyToRemove { get; init; }
        bool? RemoveAllEffects { get; init; }
    }
}