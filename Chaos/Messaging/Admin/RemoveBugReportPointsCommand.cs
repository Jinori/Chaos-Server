using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("removeBP", helpText: "<amount|targetName>")]
public class RemoveBugReportPointsCommand(IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<int>(out var amount))
        {
            // Remove bug report points from the source player
            if (source.Trackers.Counters.TryGetValue("bugreportpoints", out var currentPoints))
            {
                var newPoints = Math.Max(0, currentPoints - amount); // Ensure it doesn't go below 0
                source.Trackers.Counters.Set("bugreportpoints", newPoints);
                source.SendOrangeBarMessage($"You removed {amount} Bug Report Points. Total: {newPoints}");
            }
            else
            {
                source.SendOrangeBarMessage($"You have no Bug Report Points to remove.");
            }

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

            // Remove bug report points from the target player
            if (target.Trackers.Counters.TryGetValue("bugreportpoints", out var currentPoints))
            {
                var newPoints = Math.Max(0, currentPoints - amount); // Ensure it doesn't go below 0
                target.Trackers.Counters.Set("bugreportpoints", newPoints);
                source.SendOrangeBarMessage($"You removed {amount} Bug Report Points from {target.Name}. Total: {newPoints}");
            }
            else
            {
                source.SendOrangeBarMessage($"{target.Name} has no Bug Report Points to remove.");
            }
        }

        return default;
    }
}
