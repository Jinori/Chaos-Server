using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class RemoveEffectComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IRemoveEffectComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        if (options.RemoveAllEffects.HasValue)
        {
            foreach (var target in targets)
            {
                foreach (var effect in target.Effects)
                    target.Effects.Dispel(effect.Name);
            }
            //Now that we removed them all, let's break out
            return;
        }
        
        if (string.IsNullOrEmpty(options.EffectKey))
            return;

        foreach (var target in targets)
            target.Effects.Dispel(options.EffectKey);
    }

    public interface IRemoveEffectComponentOptions
    {
        string? EffectKey { get; init; }
        bool? RemoveAllEffects { get; init; }
    }
}