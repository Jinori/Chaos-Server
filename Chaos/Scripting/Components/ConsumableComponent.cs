using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ConsumableComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IConsumableComponentOptions>();

        context.SourceAisling?.Inventory.RemoveQuantity(options.ItemName, 1);
        context.SourceAisling?.SendOrangeBarMessage("You consumed a " + options.ItemName + ".");
    }

    public interface IConsumableComponentOptions
    {
        string ItemName { get; init; }
    }
}