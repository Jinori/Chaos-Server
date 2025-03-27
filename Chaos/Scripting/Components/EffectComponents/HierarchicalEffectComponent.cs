using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.Components.EffectComponents;

public struct HierarchicalEffectComponent : IConditionalComponent
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var target = context.TargetCreature;

        if (target is null)
            throw new InvalidOperationException("Target is null.This component is intended for use in IEffect.ShouldApply");

        var options = vars.GetOptions<IHierarchicalEffectComponentOptions>();

        // Get the rank of the current effect being applied
        var thisRank = options.EffectNameHierarchy.IndexOf(options.Name);

        // Remove all effects that have a higher rank than the new effect
        var effectsToRemove = target.Effects
                                    .Where(e => options.EffectNameHierarchy.ContainsI(e.Name))
                                    .Where(e => options.EffectNameHierarchy.IndexOf(e.Name) >= thisRank)
                                    .ToList();

        foreach (var effect in effectsToRemove)
            target.Effects.Dispel(effect.Name);

        // Check if there's any lower-ranked effect still present
        var lowerRankEffect = target.Effects
                                    .Where(e => options.EffectNameHierarchy.ContainsI(e.Name))
                                    .FirstOrDefault(e => options.EffectNameHierarchy.IndexOf(e.Name) < thisRank);

        if (lowerRankEffect != null)
        {
            context.SourceAisling?.SendActiveMessage($"Target is already under a stronger effect. [{lowerRankEffect.Name}]");

            return false;
        }

        if (target is Merchant)
            return false;

        if (!target.Effects.Contains("GM Hide") && target.Name != context.Source.Name)
            context.SourceAisling?.SendActiveMessage($"You cast {options.Name} on {target.Name}.");
        else if (target.Name == context.Source.Name)
            context.SourceAisling?.SendActiveMessage($"You cast {options.Name} on yourself.");

        if (target is Aisling aisling && target.Name != context.Source.Name)
            aisling.SendActiveMessage($"{context.Source.Name} casted {options.Name} on you.");

        return true;
    }

    public interface IHierarchicalEffectComponentOptions : IEffect
    {
        List<string> EffectNameHierarchy { get; init; }
    }
}