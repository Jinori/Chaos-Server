using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("setstat", helpText: "<stat> <amount>")]
public class SetStatCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out string? stat))
            return default;

        if (!args.TryGetNext(out int statAmount))
            return default;

        switch (stat?.ToLower())
        {
            case "str":
                source.UserStatSheet.Str = statAmount;
                break;
            case "dex":
                source.UserStatSheet.Dex = statAmount;
                break;
            case "int":
                source.UserStatSheet.Int = statAmount;
                break;
            case "wis":
                source.UserStatSheet.Wis = statAmount;
                break;
            case "con":
                source.UserStatSheet.Con = statAmount;
                break;
            default:
                // Invalid stat name.
                source.SendOrangeBarMessage("Invalid stat name. Use 'str', 'dex', 'int', 'wis', or 'con'.");
                return default;
        }

        // Send the attribute update to the client.
        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }
}
