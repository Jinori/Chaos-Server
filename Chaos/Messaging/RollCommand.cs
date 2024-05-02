using Chaos.Collections.Common;
using Chaos.Common.Utilities;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging;

[Command("rollgroup", false)]
public class RollCommand() : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        var roll = IntegerRandomizer.RollSingle(100);

        if (source.Group == null) return default;
        foreach (var member in source.Group)
            member.SendOrangeBarMessage($"{source.Name} rolled a {roll}!");

        return default;
    }
}