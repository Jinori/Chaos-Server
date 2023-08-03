using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Lava_Flow;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaUndergroundScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptFactory<IMapScript, MapInstance> ScriptFactory;
    private readonly Point LavaGreenPoint = new(23, 6);
    private readonly Point LavaRedPoint = new(8, 5);
    private readonly Point LavaBluePoint = new(8, 23);
    private readonly Point LavaGoldPoint = new(23, 21);
    private readonly Point CenterWarp = new(11,10);
    private Point CenterWarpPlayer;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    
    //Place Discord Bot Token Here When Live
    private const string BOT_TOKEN = @"MTA4Mzg2MzMyNDc3MDQzOTM1MA.GrsY_F.Wao49S7Nyz2HLtOcpIe1uJRDcDHHjq9Vm_AA2Y";
    private const ulong CHANNEL_ID = 1136412469762470038;
    private const ulong ARENA_WIN_CHANNEL_ID = 1136426304300916786;
    
    
    /// <inheritdoc />
    public ArenaUndergroundScript(Dialog subject, ISimpleCache simpleCache, IScriptFactory<IMapScript, MapInstance> scriptFactory,
        IClientRegistry<IWorldClient> clientRegistry)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        SimpleCache = simpleCache;
        ScriptFactory = scriptFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_initial":
                HideDialogOptions(source);

                break;

            case "ophie_everyonewon":
            {
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"You have won the arena event!");

                    aisling.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Arena Underground Participant",
                            "undrGrdPart",
                            MarkIcon.Victory,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));

                    aisling.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Arena Underground Win",
                            "undrGrdWin",
                            MarkIcon.Victory,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));

                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                    {
                        CenterWarpPlayer = rect.GetRandomPoint();
                    } while (CenterWarp == CenterWarpPlayer);

                    aisling.Trackers.Enums.Remove<ArenaTeam>();
                    aisling.WarpTo(CenterWarpPlayer);
                }

                var merchant = Subject.DialogSource as Merchant;
                merchant?.Say("Win is cast! Congrats!");
                
                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} cast win for every one in attendance...");

                        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                        {
                            await channel.SendMessageAsync($"{aisling.Name} won!");
                        }

                        await client.StopAsync();
                    });
                
                break;
            }
            
            case "ophie_goldwon":
            {
                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} cast a win for gold...");

                        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                            if (value == ArenaTeam.Gold)
                            {
                                await channel.SendMessageAsync($"{aisling.Name} won!");
                            }
                            else
                                await channel.SendMessageAsync($"{aisling.Name} participated!");
                        }

                        await client.StopAsync();
                    });
                
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                    if (value == ArenaTeam.Gold)
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team has won the arena event!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Win",
                                "undrGrdWin",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team lost. Better luck next time!");
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "uundrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    
                    var rect = new Rectangle(new Point(11, 10), 3, 4);
                    do
                    {
                        CenterWarpPlayer = rect.GetRandomPoint();
                    } 
                    while (CenterWarp == CenterWarpPlayer);
                    
                    aisling.Trackers.Enums.Remove<ArenaTeam>();
                    aisling.WarpTo(CenterWarpPlayer);
                }

                var merchant = Subject.DialogSource as Merchant;
                merchant?.Say("Win is cast! Congrats!");
                
                break;
            }
            
            case "ophie_bluewon":
            {
                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} cast a win for blue...");

                        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                            if (value == ArenaTeam.Blue)
                            {
                                await channel.SendMessageAsync($"{aisling.Name} won!");
                            }
                            else
                                await channel.SendMessageAsync($"{aisling.Name} participated!");
                        }

                        await client.StopAsync();
                    });
                
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                    if (value == ArenaTeam.Blue)
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team has won the arena event!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Win",
                                "undrGrdWin",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team lost. Better luck next time!");
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdWin",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    
                    var rect = new Rectangle(new Point(11, 10), 3, 4);
                    do
                    {
                        CenterWarpPlayer = rect.GetRandomPoint();
                    } 
                    while (CenterWarp == CenterWarpPlayer);
                    
                    aisling.Trackers.Enums.Remove<ArenaTeam>();
                    aisling.WarpTo(CenterWarpPlayer);
                }

                var merchant = Subject.DialogSource as Merchant;
                merchant?.Say("Win is cast! Congrats!");
                
                break;
            }
            
            case "ophie_greenwon":
            {
                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} cast a win for green...");

                        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                            if (value == ArenaTeam.Green)
                            {
                                await channel.SendMessageAsync($"{aisling.Name} won!");
                            }
                            else
                                await channel.SendMessageAsync($"{aisling.Name} participated!");
                        }

                        await client.StopAsync();
                    });
                
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                    if (value == ArenaTeam.Green)
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team has won the arena event!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Win",
                                "undrGrdWin",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team lost. Better luck next time!");
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    
                    var rect = new Rectangle(new Point(11, 10), 3, 4);
                    do
                    {
                        CenterWarpPlayer = rect.GetRandomPoint();
                    } 
                    while (CenterWarp == CenterWarpPlayer);
                    
                    aisling.Trackers.Enums.Remove<ArenaTeam>();
                    aisling.WarpTo(CenterWarpPlayer);
                    
                }

                var merchant = Subject.DialogSource as Merchant;
                merchant?.Say("Win is cast! Congrats!");
                
                break;
            }
            
            case "ophie_redwon":
            {
                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} cast a win for red...");

                        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                            if (value == ArenaTeam.Red)
                            {
                                await channel.SendMessageAsync($"{aisling.Name} won!");
                            }
                            else
                                await channel.SendMessageAsync($"{aisling.Name} participated!");
                        }

                        await client.StopAsync();
                    });
                
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam value);

                    if (value == ArenaTeam.Red)
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team has won the arena event!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Win",
                                "undrGrdWin",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Your team lost. Better luck next time!");
                        
                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }
                    
                    var rect = new Rectangle(new Point(11, 10), 3, 4);
                    do
                    {
                        CenterWarpPlayer = rect.GetRandomPoint();
                    } 
                    while (CenterWarp == CenterWarpPlayer);
                    
                    aisling.Trackers.Enums.Remove<ArenaTeam>();
                    aisling.WarpTo(CenterWarpPlayer);
                    
                }

                var merchant = Subject.DialogSource as Merchant;
                merchant?.Say("Win is cast! Congrats!");
                
                break;
            }
            
            case "ophie_skandaragauntlet":
            {
                foreach (var aisling in ClientRegistry)
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Arena Host {source.Name} is announcing a Skandara's Gauntlet!");

                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} is announcing a Skandara's Gauntlet! Please head to the Arena Underground to participate.");

                            await client.StopAsync();
                    });
                
                break;
            }
            
            case "ophie_serendaelsandbox":
            {
                foreach (var aisling in ClientRegistry)
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Arena Host {source.Name} is announcing a Serendael's Sandbox!");

                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync($"Arena Host {source.Name} is announcing a Serendael's Sandbox! Please head to the Arena Underground to participate.");

                        await client.StopAsync();
                    });
                
                break;
            }
            
            case "ophie_startffalavaflowhostnotplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowFFAHostNotPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowFFAHostNotPlayingScript), ScriptFactory);
                
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;
                    
                    do 
                        point = mapInstance.Template.Bounds.GetRandomPoint();
                    while (mapInstance.IsWall(point) || mapInstance.IsBlockingReactor(point));
                    
                    aisling.TraverseMap(mapInstance, point);
                }
                
                source.TraverseMap(mapInstance, new Point(14, 14));
                Subject.Close(source);
                break;
            }

            case "ophie_startffalavaflowhostplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowFFAHostPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowFFAHostPlayingScript), ScriptFactory);

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;

                    do
                        point = mapInstance.Template.Bounds.GetRandomPoint();
                    while (mapInstance.IsWall(point) || mapInstance.IsBlockingReactor(point));
                    
                    aisling.TraverseMap(mapInstance, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "ophie_startteamgamelavaflowhostplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowTeamsHostPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowTeamsHostPlayingScript), ScriptFactory);

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    if (!aisling.IsAlive)
                    {
                        aisling.IsDead = false;
                        aisling.StatSheet.SetHealthPct(100);
                        aisling.StatSheet.SetManaPct(100);
                        aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    }
                    
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

                    switch (team)
                    {
                        case ArenaTeam.Blue:
                            aisling.TraverseMap(mapInstance, LavaBluePoint);
                            break;
                        case ArenaTeam.Green:
                            aisling.TraverseMap(mapInstance, LavaGreenPoint);
                            break;
                        case ArenaTeam.Gold:
                            aisling.TraverseMap(mapInstance, LavaGoldPoint);
                            break;
                        case ArenaTeam.Red:
                            aisling.TraverseMap(mapInstance, LavaRedPoint);
                            break;
                    }
                }
                Subject.Close(source);
                break;
            }
            
            case "ophie_startteamgamelavaflowhostnotplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowTeamsHostNotPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowTeamsHostNotPlayingScript), ScriptFactory);

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    if (!aisling.IsAlive)
                    {
                        aisling.IsDead = false;
                        aisling.StatSheet.SetHealthPct(100);
                        aisling.StatSheet.SetManaPct(100);
                        aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    }
                    
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

                    switch (team)
                    {
                        case ArenaTeam.Blue:
                            aisling.TraverseMap(mapInstance, LavaBluePoint);
                            break;
                        case ArenaTeam.Green:
                            aisling.TraverseMap(mapInstance, LavaGreenPoint);
                            break;
                        case ArenaTeam.Gold:
                            aisling.TraverseMap(mapInstance, LavaGoldPoint);
                            break;
                        case ArenaTeam.Red:
                            aisling.TraverseMap(mapInstance, LavaRedPoint);
                            break;
                    }
                }
                
                source.TraverseMap(mapInstance, new Point(14, 14));
                Subject.Close(source);
                break;
            }
            case "ophie_battlering":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(13, 13));
                Subject.Close(source);

                break;
            }

            case "ophie_revive":
            {
                Subject.Close(source);

                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(100);
                    source.StatSheet.SetManaPct(100);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("Ophie has revived you.");
                    source.Refresh();
                }

                break;
            }

            case "ophie_leave":
            {
                Subject.Close(source);
                var worldMap = SimpleCache.Get<WorldMap>("field001");

                //if we cant set the active object, return
                if (!source.ActiveObject.SetIfNull(worldMap))
                    return;

                source.MapInstance.RemoveObject(source);
                source.Client.SendWorldMap(worldMap);

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_placeonredconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Red);
                var rect = new Rectangle(new Point(11, 17), 4, 3);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Red team!");

                break;
            }
            case "ophie_placeongreenconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Green);
                var rect = new Rectangle(new Point(18, 10), 3, 4);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Green team!");

                break;
            }
            case "ophie_placeongoldconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Gold);
                var rect = new Rectangle(new Point(5, 10), 4, 3);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Gold team!");

                break;
            }
            case "ophie_placeonblueconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Blue);
                var rect = new Rectangle(new Point(12, 4), 4, 3);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Blue team!");

                break;
            }
            case "ophie_removeteamconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }
                
                var rect = new Rectangle(new Point(11, 10), 3, 4);
                    
                do
                {
                    CenterWarpPlayer = rect.GetRandomPoint();
                } 
                while (CenterWarp == CenterWarpPlayer);
                
                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Remove<ArenaTeam>();
                aisling?.WarpTo(CenterWarpPlayer);
                break;
            }
        }
    }
    
    
    public void HideDialogOptions(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ArenaHost stage);   
        if ((stage != ArenaHost.Host) && (stage != ArenaHost.MasterHost))
            RemoveOption(Subject, "Host Options"); 
    }
    
    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName).HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }
}