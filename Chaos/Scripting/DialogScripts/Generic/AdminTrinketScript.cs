using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Generic;

public class AdminTrinketScript : DialogScriptBase
{
    private const string BOT_TOKEN = @"MTA4Mzg2MzMyNDc3MDQzOTM1MA.Gek2EZ.se-vh-fRnr78vAYuub5KUK-aIzrUHEMlEkB75Y";
    private readonly ISimpleCache _cache;
    private readonly ISimpleCacheProvider _cacheProvider;
    private readonly ulong _channelId = 1089331247999885372;

    private readonly IClientRegistry<IChaosWorldClient> _clientRegistry;
    private readonly ISimpleCache SimpleCache;

    public AdminTrinketScript(
        Dialog subject,
        ISimpleCache cache,
        ISimpleCacheProvider cacheProvider,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        _cacheProvider = cacheProvider;
        _cache = cache;
        _clientRegistry = clientRegistry;
        SimpleCache = simpleCache;
    }

    private Aisling? GetPlayerByName(string playerName) =>
        _clientRegistry
            .Select(c => c.Aisling)
            .FirstOrDefault(a => a.Name.EqualsI(playerName));

    public override void OnDisplaying(Aisling source)
    {
        if (!source.IsAdmin)
        {
            source.SendOrangeBarMessage("You are not an admin.");

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
            source.SendOrangeBarMessage("You are not an admin.");

            return;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "admintrinket_adminmessage":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var message))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var player = _clientRegistry
                             .Select(c => c.Aisling)
                             .Where(_ => true);

                foreach (var aisling in player)
                    aisling.SendOrangeBarMessage($"[{source.Name}]: {message}.");

                Subject.Close(source);

                break;
            }

            case "admintrinket_teleport":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var playerName))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var player = GetPlayerByName(playerName);

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
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                _cacheProvider.GetCache<MapInstance>();

                var player = GetPlayerByName(playerName);

                if (player == null)
                {
                    source.SendOrangeBarMessage($"{playerName} is not online");
                    Subject.Close(source);

                    return;
                }

                player.TraverseMap(source.MapInstance, source, true);
                source.SendOrangeBarMessage($"You teleported {playerName} to you.");
                Subject.Close(source);
                ReportActivity(source, player, "Summon");

                return;
            }

            case "admintrinket_jailplayer":
            {
                if (!Subject.MenuArgs.TryGetNext<string>(out var playerName))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var mapInstance = _cache.Get<MapInstance>("loures_underground_jail");

                var player = GetPlayerByName(playerName);

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

    private void ReportActivity(Aisling source, Aisling target, string command) =>
        Task.Run(
            async () =>
            {
                var client = new DiscordSocketClient();
                await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                await client.StartAsync();
                var channel = await client.GetChannelAsync(_channelId) as IMessageChannel;

                await channel!.SendMessageAsync(
                    $"```" + $"Admin {source.Name} has used the {command} command on Player {target.Name}" + "```");

                await client.StopAsync();
            });
}