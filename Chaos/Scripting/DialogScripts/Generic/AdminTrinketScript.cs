using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Generic;

public class AdminTrinketScript : DialogScriptBase
{
    private const string BOT_TOKEN = @"";
    private readonly ISimpleCache Cache;
    private readonly ISimpleCacheProvider CacheProvider;
    private const ulong CHANNEL_ID = 1089331247999885372;

    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly ISimpleCache SimpleCache;
    private readonly IMonsterFactory MonsterFactory;

    public AdminTrinketScript(
        Dialog subject,
        ISimpleCache cache,
        ISimpleCacheProvider cacheProvider,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        ISimpleCache simpleCache,
        IMonsterFactory monsterFactory
    )
        : base(subject)
    {
        CacheProvider = cacheProvider;
        Cache = cache;
        ClientRegistry = clientRegistry;
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;
    }

    private Aisling? GetPlayerByName(string playerName) =>
        ClientRegistry
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
                DisplayMapInfo(source);
                break;
        }
    }

    private void DisplayMapInfo(Aisling source)
    {
        var mapInstance = SimpleCache.Get<MapInstance>(source.MapInstance.InstanceId);
        Subject.Close(source);

        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            $"Name: {mapInstance.Name}\nTemplateKey: {mapInstance.Template.TemplateKey}\nMusic: {mapInstance.Music}\nInstanceId: {mapInstance.InstanceId}\nMax Level: {mapInstance.MaximumLevel}\nMin Level: {mapInstance.MinimumLevel}\nShard: {mapInstance.IsShard}\nX: {mapInstance.Template.Height}\nY: {mapInstance.Template.Width}");
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
            case "admintrinket_spawn97generaterandom":
                SpawnMobs(source, "random", 10, 97);
                break;

            case "admintrinket_spawn97darkcarnunconfirm":
                HandleMobSpawnRequest(source, "arenacarnun97", 97);
                break;

            case "admintrinket_spawn97darkmonkconfirm":
                HandleMobSpawnRequest(source, "arenamonk97", 97);
                break;

            case "admintrinket_spawn97darkwarriorconfirm":
                HandleMobSpawnRequest(source, "arenawarrior97", 97);
                break;

            case "admintrinket_spawn97darkwizardconfirm":
                HandleMobSpawnRequest(source, "arenawizard97", 97);
                break;

            case "admintrinket_spawn71generaterandom":
                SpawnMobs(source, "random", 10, 71);
                break;

            case "admintrinket_spawn71darkcarnunconfirm":
                HandleMobSpawnRequest(source, "arenacarnun71", 71);
                break;

            case "admintrinket_spawn71darkmonkconfirm":
                HandleMobSpawnRequest(source, "arenamonk71", 71);
                break;

            case "admintrinket_spawn71darkrogueconfirm":
                HandleMobSpawnRequest(source, "arenarogue71", 71);
                break;

            case "admintrinket_spawn71darkwizardconfirm":
                HandleMobSpawnRequest(source, "arenawizard71", 71);
                break;

            case "admintrinket_spawn41generaterandom":
                SpawnMobs(source, "random", 10, 41);
                break;

            case "admintrinket_spawn41darkmonkconfirm":
                HandleMobSpawnRequest(source, "arenamonk41", 41);
                break;

            case "admintrinket_spawn41darkwarriorconfirm":
                HandleMobSpawnRequest(source, "arenawarrior41", 41);
                break;

            case "admintrinket_spawn41darkwizardconfirm":
                HandleMobSpawnRequest(source, "arenawizard41", 41);
                break;

            case "admintrinket_adminmessage":
                HandleAdminMessage(source);
                break;

            case "admintrinket_teleport":
                HandleTeleportRequest(source);
                break;

            case "admintrinket_summonplayer":
                HandleSummonPlayerRequest(source);
                break;

            case "admintrinket_jailplayer":
                HandleJailPlayerRequest(source);
                break;
        }
    }
    
    private void HandleMobSpawnRequest(Aisling source, string mobType, int level)
    {
        if (!Subject.MenuArgs.TryGetNext<int>(out var quantity))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        SpawnMobs(source, mobType, quantity, level);
    }


    private void SpawnMobs(Aisling source, string mobType, int quantity, int level)
    {
        var rectangle = new Rectangle(source, 5, 5);
        for (var i = 0; i < quantity; i++)
        {
            if (!rectangle.TryGetRandomPoint(x => source.MapInstance.IsWalkable(x, source.Type), out var point))
                continue;

            var mobName = mobType == "random" ? GetRandomDarkMob(level) : mobType;
            var mob = MonsterFactory.Create(mobName, source.MapInstance, point);
            source.MapInstance.AddEntity(mob, point);
        }
    }

    private static string GetRandomDarkMob(int level)
    {
        var darkMobs = level switch
        {
            41 => new[] { "arenamonk41", "arenawarrior41", "arenawizard41" },
            71 => new[] { "arenamonk71", "arenawarrior71", "arenawizard71" },
            97 => new[] { "arenamonk97", "arenawarrior97", "arenawizard97" },
            _ => new[] { "arenamonk41", "arenawarrior41", "arenawizard41" }
        };

        var random = new Random();
        return darkMobs[random.Next(darkMobs.Length)];
    }

    private void HandleAdminMessage(Aisling source)
    {
        if (!Subject.MenuArgs.TryGetNext<string>(out var message))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var players = ClientRegistry.Select(c => c.Aisling);
        foreach (var player in players)
        {
            player.SendOrangeBarMessage($"[{source.Name}]: {message}.");
        }
        Subject.Close(source);
    }

    private void HandleTeleportRequest(Aisling source)
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
    }

    private void HandleSummonPlayerRequest(Aisling source)
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
            Subject.Close(source);
            return;
        }

        player.TraverseMap(source.MapInstance, source, true);
        source.SendOrangeBarMessage($"You teleported {playerName} to you.");
        ReportActivity(source, player, "Summon");
        Subject.Close(source);
    }

    private void HandleJailPlayerRequest(Aisling source)
    {
        if (!Subject.MenuArgs.TryGetNext<string>(out var playerName))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var mapInstance = Cache.Get<MapInstance>("loures_underground_jail");
        var player = GetPlayerByName(playerName);

        if (player == null)
        {
            source.SendOrangeBarMessage($"{playerName} is not online");
            return;
        }

        player.TraverseMap(mapInstance, new Point(3, 1), true);
        player.SendOrangeBarMessage("You have been jailed for four hours.");
        player.Legend.AddOrAccumulate(new LegendMark($"Arrested by {source.Name}", "arrested", MarkIcon.Victory, MarkColor.Orange, 1, GameTime.Now));
        player.Trackers.TimedEvents.AddEvent("Jail", TimeSpan.FromHours(4), true);
        source.SendOrangeBarMessage($"You have jailed {playerName}.");
        ReportActivity(source, player, "Jail");
        Subject.Close(source);
    }

    private void ReportActivity(Aisling source, Aisling target, string command) =>
        Task.Run(async () =>
        {
            var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
            await client.StartAsync();
            var channel = await client.GetChannelAsync(CHANNEL_ID) as IMessageChannel;

            await channel!.SendMessageAsync($"```Admin {source.Name} has used the {command} command on Player {target.Name}```");

            await client.StopAsync();
        });
}
