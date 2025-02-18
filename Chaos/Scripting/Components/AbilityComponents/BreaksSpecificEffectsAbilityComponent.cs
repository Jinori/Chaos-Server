using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct BreaksSpecificEffectsAbilityComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IBreaksSpecificEffectsComponentOptions>();

        options.EffectKeysToBreak ??= [];

        foreach (var effectKey in options.EffectKeysToBreak)
            context.Source.Effects.Dispel(effectKey);
    }

    public interface IBreaksSpecificEffectsComponentOptions
    {
        List<string>? EffectKeysToBreak { get; set; }
    }
}