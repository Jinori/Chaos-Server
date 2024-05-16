using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ConsumableAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IConsumableComponentOptions>();

        context.SourceAisling?.Inventory.RemoveQuantity(options.ItemName, 1);

        if (options.Message)
            context.SourceAisling?.SendOrangeBarMessage("You consumed a " + options.ItemName + ".");
    }

    public interface IConsumableComponentOptions
    {
        string ItemName { get; init; }

        bool Message { get; init; }
    }
}