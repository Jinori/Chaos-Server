using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Commands.Admin;

[Command("jail")]
public sealed class JailCommand : ICommand<Aisling>
{
    private readonly ISimpleCache Cache;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public JailCommand(ISimpleCache cache, IClientRegistry<IWorldClient> clientRegistry)
    {
        Cache = cache;
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling aisling, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var playerName))
            return default;
        
        var mapInstance = Cache.Get<MapInstance>("loures_underground_jail");
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var player = ClientRegistry
            .Select(c => c.Aisling)
            .Where(a => a != null)
            .FirstOrDefault(a => a.Name.EqualsI(playerName));

        if (player == null)
        {
            aisling.SendOrangeBarMessage($"{playerName} is not online");
            return default;
        }
        
        player.TraverseMap(mapInstance, new Point(3, 1), true);
        player.SendOrangeBarMessage("You have been jailed for four hours.");
        player.Legend.AddOrAccumulate(new LegendMark($"Arrested by {aisling.Name}", "arrested", MarkIcon.Victory, MarkColor.Orange, 1, GameTime.Now));
        player.Trackers.TimedEvents.AddEvent("Jail", TimeSpan.FromHours(4), true);
        return default;
    }
}