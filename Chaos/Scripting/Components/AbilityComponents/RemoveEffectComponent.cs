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
                    if (effect.Name == "Fury1")
                        continue;

                    if (effect.Name == "Fury2")
                        continue;

                    if (effect.Name == "Fury3")
                        continue;

                    if (effect.Name == "Fury4")
                        continue;

                    if (effect.Name == "Fury5")
                        continue;

                    if (effect.Name == "Fury6")
                        continue;
                    
                    if (effect.Name == "Cunning1")
                        continue;

                    if (effect.Name == "Cunning2")
                        continue;

                    if (effect.Name == "Cunning3")
                        continue;

                    if (effect.Name == "Cunning4")
                        continue;

                    if (effect.Name == "Cunning5")
                        continue;

                    if (effect.Name == "Cunning6")
                        continue;

                    if (effect.Name == "Invulnerability")
                        continue;

                    if (effect.Name == "mount")
                        continue;

                    if (effect.Name == "GM Knowledge")
                        continue;

                    if (effect.Name == "Strong Knowledge")
                        continue;

                    if (effect.Name == "Stoned")
                        continue;

                    if (effect.Name == "Knowledge")
                        continue;

                    if (effect.Name == "Werewolf")
                        continue;

                    if (effect.Name == "Fishing")
                        continue;

                    if (effect.Name == "Foraging")
                        continue;

                    target.Effects.Dispel(effect.Name);
                }
            }

            // Exit after removing all effects
            return;
        }

        // If NegativeEffect is set to true, remove specific negative effects
        if (options.NegativeEffect == true)
        {
            var negativeEffects = new[]
            {
                "Poison",
                "Blind",
                "Burn"
            };

            foreach (var target in targets)
            {
                foreach (var effect in negativeEffects)
                    target.Effects.Dispel(effect);
            }

            // Exit after removing negative effects
            return;
        }

        // If EffectKey is provided, remove specific effect
        if (!string.IsNullOrEmpty(options.EffectKey))
            foreach (var target in targets)
                target.Effects.Dispel(options.EffectKey);
    }

    public interface IRemoveEffectComponentOptions
    {
        string? EffectKey { get; init; }
        bool? NegativeEffect { get; init; } // Add NegativeEffect option
        bool? RemoveAllEffects { get; init; }
    }
}