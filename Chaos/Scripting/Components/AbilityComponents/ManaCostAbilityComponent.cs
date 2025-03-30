using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ManaCostAbilityComponent : IConditionalComponent
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaCostComponentOptions>();

        // Base mana cost
        var baseCost = options.ManaCost ?? 0;

        // Add percentage-based mana cost if specified
        baseCost += MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, options.PctManaCost);

        // Calculate mana cost based on the number of targets hit
        var totalCost = baseCost; // Full cost for the first target

        // Check if the source has enough mana
        if (!context.Source.StatSheet.TrySubtractMp(totalCost))
        {
            context.SourceAisling?.SendActiveMessage("You do not have enough mana.");

            return false;
        }

        // Update the client's mana display
        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);

        return true;
    }

    public interface IManaCostComponentOptions
    {
        int? ManaCost { get; init; } // Base mana cost
        decimal PctManaCost { get; init; } // Percentage-based mana cost
    }
}