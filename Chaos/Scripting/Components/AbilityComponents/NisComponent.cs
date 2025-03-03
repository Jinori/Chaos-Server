using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Time;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class NisComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<INisComponentOptions>();
        
        if (options.OutputType != null)
        {
            var gameTime = GameTime.Now.GetDetailedTimeInfo();
            context.SourceAisling?.SendServerMessage(options.OutputType.Value, gameTime);
        }
    }

    public interface INisComponentOptions
    {
        ServerMessageType? OutputType { get; init; }
    }
}
