using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.Components.EffectComponents;

public class NonOverwritableEffectComponent : IConditionalComponent
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var target = context.TargetCreature;

        if (target is null)
            throw new InvalidOperationException("Target is null. This component is intended for use in IEffect.ShouldApply");

        var options = vars.GetOptions<INonOverwritableEffectComponentOptions>();

        var conflictingEffect = target.Effects.FirstOrDefault(e => options.ConflictingEffectNames.ContainsI(e.Name));

        if (conflictingEffect is null)
        {
            if (target is Aisling aisling)
                if (context.SourceAisling?.Name != aisling.Name && (options.Name != "GM Knowledge"))
                {
                    context.SourceAisling?.SendActiveMessage($"You cast {options.Name} on {aisling.Name}.");
                    aisling.SendActiveMessage($"{context.Source.Name} casted {options.Name} on you.");
                } else if ((options.Name != "Mount") && (options.Name != "GM Knowledge"))
                    aisling.SendActiveMessage($"You casted {options.Name} on yourself.");

            return true;
        }

        context.SourceAisling?.SendActiveMessage($"Target is already under another effect. [{conflictingEffect.Name}]");

        return false;
    }

    public interface INonOverwritableEffectComponentOptions : IEffect
    {
        List<string> ConflictingEffectNames { get; init; }
    }
}