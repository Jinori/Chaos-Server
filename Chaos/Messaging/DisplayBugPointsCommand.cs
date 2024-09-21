using Chaos.Collections.Common;
using Chaos.Common.Utilities;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging;

[Command("displaybp", false)]
public class DisplayBugPointsCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        // Fetch the player's bug points from their trackers
        if (!source.Trackers.Counters.TryGetValue("bugreportpoints", out var bugPoints))
        {
            bugPoints = 0; // Default to 0 if the tracker doesn't exist
        }
      
        source.SendOrangeBarMessage($"You have {bugPoints} Bug Points.");

        return default;
    }
}