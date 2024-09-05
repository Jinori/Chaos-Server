using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class RemoveEffectComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IRemoveEffectComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        // If RemoveAllEffects is set to true, remove all effects
        if (options.RemoveAllEffects == true)
        {
            foreach (var target in targets)
            {
                foreach (var effect in target.Effects.ToList()) // ToList() to avoid modification during iteration
                {
                    target.Effects.Dispel(effect.Name);
                }
            }
            // Exit after removing all effects
            return;
        }

        // If NegativeEffect is set to true, remove specific negative effects
        if (options.NegativeEffect == true)
        {
            var negativeEffects = new[] { "Poison", "Blind", "Burn" };
            foreach (var target in targets)
            {
                foreach (var effect in negativeEffects)
                {
                    target.Effects.Dispel(effect);
                }
            }
            // Exit after removing negative effects
            return;
        }

        // If EffectKey is provided, remove specific effect
        if (!string.IsNullOrEmpty(options.EffectKey))
        {
            foreach (var target in targets)
            {
                target.Effects.Dispel(options.EffectKey);
            }
        }
    }

    public interface IRemoveEffectComponentOptions
    {
        string? EffectKey { get; init; }
        bool? RemoveAllEffects { get; init; }
        bool? NegativeEffect { get; init; } // Add NegativeEffect option
    }
}