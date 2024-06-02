using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Messaging.Admin;

[Command("jail")]
public sealed class JailCommand : ICommand<Aisling>
{
    private readonly ISimpleCache Cache;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

    public JailCommand(ISimpleCache cache, IClientRegistry<IChaosWorldClient> clientRegistry)
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