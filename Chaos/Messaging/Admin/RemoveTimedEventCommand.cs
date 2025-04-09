using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("removeTimedEvent", helpText: "<name>, <eventId>")]
public class RemoveTimedEventCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    public RemoveTimedEventCommand(IClientRegistry<IChaosWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var name))
            return default;

        if (!args.TryGetNext<string>(out var eventId))
            return default;

        var client = ClientRegistry.FirstOrDefault(client => client.Aisling.Name.EqualsI(name));

        if (client is null)
            return default;

        try
        {
            client.Aisling.Trackers.TimedEvents.ForceRemoveEvent(eventId);
            
            source.SendOrangeBarMessage("Timed event removed.");
        } catch (Exception e)
        {
            source.SendOrangeBarMessage($"Failed to remove event. {e.Message}");
        }

        return default;
    }
}