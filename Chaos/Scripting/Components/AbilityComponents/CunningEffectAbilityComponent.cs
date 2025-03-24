using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

/// <summary>
/// Handles the application of "Cunning" effects based on the target's mana threshold.
/// </summary>
public struct CunningEffectAbilityComponent : IComponent
{
    private readonly IEffectFactory _effectFactory;

    public CunningEffectAbilityComponent(IEffectFactory effectFactory) => _effectFactory = effectFactory ?? throw new ArgumentNullException(nameof(effectFactory));

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();

        // Define the Cunning effects and their corresponding MP thresholds.
        var cunningEffects = new Dictionary<string, int>
        {
            { "Cunning1", 4000 },
            { "Cunning2", 8000 },
            { "Cunning3", 16000 },
            { "Cunning4", 32000 },
            { "Cunning5", 64000 },
            { "Cunning6", 128000 }
        };

        foreach (var target in targets)
        {
            ApplyCunningEffect(target, context, cunningEffects);
        }
    }

    /// <summary>
    /// Applies the appropriate "Cunning" effect based on the target's current MP.
    /// </summary>
    private void ApplyCunningEffect(Creature target, ActivationContext context, Dictionary<string, int> cunningEffects)
    {
        var currentMp = target.StatSheet.EffectiveMaximumMp;
        var activeEffect = cunningEffects.Keys.FirstOrDefault(target.Effects.Contains);

        // Determine the next effect to apply
        foreach ((var effect, var mpThreshold) in cunningEffects)
        {
            if (currentMp < mpThreshold) break;

            if (activeEffect == null)
            {
                target.Effects.Apply(context.Source, _effectFactory.Create(effect));
                return;
            }

            if (effect == activeEffect)
            {
                target.Effects.Terminate(effect);
                continue;
            }

            if (cunningEffects.TryGetValue(activeEffect, out var value) && (value < mpThreshold))
            {
                target.Effects.Apply(context.Source, _effectFactory.Create(effect));
                return;
            }
        }

        context.SourceAisling?.SendOrangeBarMessage("You do not have the mana to increase your Cunning.");
    }
}
