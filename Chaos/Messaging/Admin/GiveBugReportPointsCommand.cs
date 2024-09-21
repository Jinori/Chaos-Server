using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("giveBP", helpText: "<amount|targetName>")]
public class GiveBugReportPointsCommand(IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<int>(out var amount))
        {
            // Add bug report points to the source player
            source.Trackers.Counters.AddOrIncrement("bugreportpoints", amount);
            source.Trackers.Counters.TryGetValue("bugreportpoints", out var totalamount);
            source.SendOrangeBarMessage($"You gave yourself {amount} Bug Report Points. Total: {totalamount}");

            return default;
        }

        if (args.TryGetNext<string>(out var targetName) && args.TryGetNext(out amount))
        {
            var target = clientRegistry.Select(client => client.Aisling)
                .FirstOrDefault(aisling => aisling.Name.EqualsI(targetName));

            if (target == null)
            {
                source.SendOrangeBarMessage($"{targetName} is not online");
                return default;
            }

            // Add bug report points to the target player
            target.Trackers.Counters.AddOrIncrement("bugreportpoints", amount);
            target.Trackers.Counters.TryGetValue("bugreportpoints", out var totalamount);
            target.SendOrangeBarMessage($"{source.Name} gave you {amount} Bug Report Points. Total: {totalamount}");
            source.SendOrangeBarMessage($"You gave {target.Name} {amount} Bug Report Points. Total: {totalamount}");
        }

        return default;
    }
}