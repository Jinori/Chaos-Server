using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct HealthCostAbilityComponent : IConditionalComponent
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IHealthCostComponentOptions>();

        // Base health cost
        var baseCost = options.HealthCost ?? 0;

        // Add percentage-based health cost if specified
        baseCost += MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, options.PctHealthCost);

        // Calculate health cost based on the number of targets hit
        var totalCost = baseCost; // Full cost for the first target

        // Check if the source has enough health
        if (!context.Source.StatSheet.TrySubtractHp(totalCost))
        {
            context.SourceAisling?.SendActiveMessage("You do not have enough health.");

            return false;
        }

        // Update the client's health display
        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);

        return true;
    }

    public interface IHealthCostComponentOptions
    {
        int? HealthCost { get; init; } // Base health cost
        decimal PctHealthCost { get; init; } // Percentage-based health cost
    }
}