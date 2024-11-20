using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
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

        // Adjust each stat based on comparison with target value
        switch (stat?.ToLower())
        {
            case "str":
                var str = new Attributes
                {
                    Str = CalculateAdjustment(source.UserStatSheet.Str, statAmount)
                };
                source.UserStatSheet.Add(str);
                break;

            case "dex":
                var dex = new Attributes
                {
                    Dex = CalculateAdjustment(source.UserStatSheet.Dex, statAmount)
                };
                source.UserStatSheet.Add(dex);
                break;

            case "int":
                var @int = new Attributes
                {
                    Int = CalculateAdjustment(source.UserStatSheet.Int, statAmount)
                };
                source.UserStatSheet.Add(@int);
                break;

            case "wis":
                var wis = new Attributes
                {
                    Wis = CalculateAdjustment(source.UserStatSheet.Wis, statAmount)
                };
                source.UserStatSheet.Add(wis);
                break;

            case "con":
                var con = new Attributes
                {
                    Con = CalculateAdjustment(source.UserStatSheet.Con, statAmount)
                };
                source.UserStatSheet.Add(con);
                break;

            case "all":
                var all = new Attributes
                {
                    Str = CalculateAdjustment(source.UserStatSheet.Str, statAmount),
                    Int = CalculateAdjustment(source.UserStatSheet.Int, statAmount),
                    Wis = CalculateAdjustment(source.UserStatSheet.Wis, statAmount),
                    Con = CalculateAdjustment(source.UserStatSheet.Con, statAmount),
                    Dex = CalculateAdjustment(source.UserStatSheet.Dex, statAmount)
                };
                source.UserStatSheet.Add(all);
                break;

            default:
                source.SendOrangeBarMessage("Invalid stat name. Use 'str', 'dex', 'int', 'wis', 'con', or 'all'");
                return default;
        }

        // Send the attribute update to the client.
        source.Client.SendAttributes(StatUpdateType.Full);

        return default;

        // Calculate the exact adjustment needed to reach the target amount
        static int CalculateAdjustment(int currentStat, int targetAmount) => targetAmount - currentStat;
    }
}