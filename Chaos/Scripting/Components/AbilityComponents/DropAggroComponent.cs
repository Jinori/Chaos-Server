using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class DropAggroComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();
        var options = vars.GetOptions<IDropAggroComponentOptions>();

        foreach (var target in targets)
        {
            if (target is Aisling or Merchant)
                break;

            var monster = target as Monster;

            if (options.AllAggro)
                monster?.ResetAggro();
        }
    }

    public interface IDropAggroComponentOptions
    {
        bool AllAggro { get; set; }
    }
}