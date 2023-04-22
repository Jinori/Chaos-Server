using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Utilities;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Generic;

public class AdminTrinketScript : DialogScriptBase
{

    private readonly IClientRegistry<IWorldClient> _clientRegistry;
    private readonly ISimpleCache SimpleCache;
    private readonly ISimpleCacheProvider _cacheProvider;
    private readonly ISimpleCache _cache;

    public AdminTrinketScript(
        Dialog subject,
        ISimpleCache cache,
        ISimpleCacheProvider cacheProvider,
        IClientRegistry<IWorldClient> clientRegistry,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        _cacheProvider = cacheProvider;
        _cache = cache;
        _clientRegistry = clientRegistry;
        SimpleCache = simpleCache;
    }

    const string BotToken = @"";
    readonly ulong _channelId = 1089331247999885372;

    private void ReportActivity(Aisling source, Aisling target, string command)
    {
        Task.Run(
            async () =>
            {
                var client = new DiscordSocketClient();
                await client.LoginAsync(TokenType.Bot, BotToken);
                await client.StartAsync();
                var channel = await client.GetChannelAsync(_channelId) as IMessageChannel;

                await channel!.SendMessageAsync(
                    $"```" + $"Admin {source.Name} has used the {command} command on Player {target.Name}" + "```");

                await client.StopAsync();
            });
    }

    public override void OnDisplaying(Aisling source)
    {
        if (!source.IsAdmin)
        {
            source.SendOrangeBarMessage($"You are not an admin.");

            return;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "admintrinket_displaymapinfo":
            {
                var mapInstance = SimpleCache.Get<MapInstance>(source.MapInstance.InstanceId);
                Subject.Close(source);

                source.Client.SendServerMessage(
                    ServerMessageType.ScrollWindow,
                    "Name: "
                    + mapInstance.Name
                    + "\nTemplateKey: "
                    + mapInstance.Template.TemplateKey
                    + "\nMusic: "
                    + mapInstance.Music
                    + "\nInstanceId: "
                    + mapInstance.InstanceId
                    + "\nMax Level: "
                    + mapInstance.MaximumLevel
                    + "\nMin Level: "
                    + mapInstance.MinimumLevel
                    + "\nShard: "
                    + mapInstance.IsShard
                    + "\nX: "
                    + mapInstance.Template.Height
                    + "\nY: "
                    + mapInstance.Template.Width);

                return;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!source.IsAdmin)
        {
            source.SendOrangeBarMessage($"You are not an admin.");

            return;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "admintrinket_adminmessage":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var message))
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);

                    return;
                }

                var player = _clientRegistry
                             .Select(c => c.Aisling)
                             .Where(a => true);

                foreach (var aisling in player)
                {
                    aisling.SendOrangeBarMessage($"[{source.Name}]: {message}.");
                }

                Subject.Close(source);

                break;
            }

            case "admintrinket_teleport":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var playerName))
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);

                    return;
                }

                var player = _clientRegistry
                             .Select(c => c.Aisling)
                             .Where(a => a != null)
                             .FirstOrDefault(a => a.Name.EqualsI(playerName));

                if (player == null)
                {
                    source.SendOrangeBarMessage($"{playerName} is not online");

                    return;
                }

                source.TraverseMap(player.MapInstance, player, true);
                source.SendOrangeBarMessage($"You teleported to {playerName}.");
                ReportActivity(source, player, "Teleport");
                Subject.Close(source);

                break;
            }

            case "admintrinket_summonplayer":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var playerName))
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);

                    return;
                }

                var mapCache = _cacheProvider.GetCache<MapInstance>();

                var aisling = mapCache.SelectMany(map => map.GetEntities<Aisling>())
                                      .FirstOrDefault(aisling => aisling.Name.EqualsI(playerName));

                if (aisling == null)
                {
                    source.SendOrangeBarMessage($"{playerName} is not online");
                    Subject.Close(source);

                    return;
                }

                aisling.TraverseMap(source.MapInstance, source, true);
                source.SendOrangeBarMessage($"You teleported {playerName} to you.");
                Subject.Close(source);
                ReportActivity(source, aisling, "Summon");

                return;
            }

            case "admintrinket_jailplayer":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var playerName))
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);

                    return;
                }

                var mapInstance = _cache.Get<MapInstance>("loures_underground_jail");

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                var player = _clientRegistry
                             .Select(c => c.Aisling)
                             .Where(a => a != null)
                             .FirstOrDefault(a => a.Name.EqualsI(playerName));

                if (player == null)
                {
                    source.SendOrangeBarMessage($"{playerName} is not online");

                    return;
                }

                player.TraverseMap(mapInstance, new Point(3, 1), true);
                player.SendOrangeBarMessage("You have been jailed for four hours.");

                player.Legend.AddOrAccumulate(
                    new LegendMark(
                        $"Arrested by {source.Name}",
                        "arrested",
                        MarkIcon.Victory,
                        MarkColor.Orange,
                        1,
                        GameTime.Now));

                player.Trackers.TimedEvents.AddEvent("Jail", TimeSpan.FromHours(4), true);
                source.SendOrangeBarMessage($"You have jailed {playerName}.");
                Subject.Close(source);
                ReportActivity(source, player, "Jail");

                return;
            }
        }
    }
}