using Chaos.Collections.Common;
using Chaos.Common.Utilities;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging;

[Command("rolloutloud", false)]
public class RollOutloudCommand() : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {        
        var roll = IntegerRandomizer.RollSingle(100);
        
        foreach (var member in source.MapInstance.GetEntitiesWithinRange<Aisling>(source))
            member.SendOrangeBarMessage($"{source.Name} rolled a {roll}!");

        return default;
    }
}