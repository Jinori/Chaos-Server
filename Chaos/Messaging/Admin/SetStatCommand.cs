using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
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
                var str = new Attributes
                {
                    Str = source.UserStatSheet.Str - statAmount
                };
                
                source.UserStatSheet.Add(str);
                break;
            case "dex":
                var dex = new Attributes
                {
                    Dex = source.UserStatSheet.Dex - statAmount
                };
                
                source.UserStatSheet.Add(dex);
                break;
            case "int":
                var @int = new Attributes
                {
                    Int = source.UserStatSheet.Int - statAmount
                };
                
                source.UserStatSheet.Add(@int);
                break;
            case "wis":
                var wis  = new Attributes
                {
                    Wis= source.UserStatSheet.Wis - statAmount
                };
                
                source.UserStatSheet.Add(wis);
                break;
            case "con":
                var con = new Attributes
                {
                   Con = source.UserStatSheet.Con - statAmount
                };
                
                source.UserStatSheet.Add(con);
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
