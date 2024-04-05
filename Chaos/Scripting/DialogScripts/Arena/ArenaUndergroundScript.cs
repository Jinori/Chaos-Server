using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MapScripts.Arena.Arena_Modes;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaUndergroundScript : DialogScriptBase
{
    //Place Discord Bot Token Here When Live
    private const string BOT_TOKEN = @"MTA4Mzg2MzMyNDc3MDQzOTM1MA.GXX0LL.48H24xZi7kJ6QZyzZn2GhKgmkoRSbgPB843q_Y";
    private const ulong CHANNEL_ID = 1136412469762470038;
    private const ulong ARENA_WIN_CHANNEL_ID = 1136426304300916786;
    private readonly Point CenterWarp = new(11, 10);
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly Point LavaBluePoint = new(8, 23);
    private readonly Point LavaGoldPoint = new(23, 21);
    private readonly Point LavaGreenPoint = new(23, 6);
    private readonly Point LavaRedPoint = new(8, 5);
    private readonly Point ColorClashBluePoint = new(27, 27);
    private readonly Point ColorClashGoldPoint = new(4, 4);
    private readonly Point ColorClashGreenPoint = new(39, 6);
    private readonly Point ColorClashRedPoint = new(4, 27);
    private readonly ISimpleCache SimpleCache;
    private readonly IShardGenerator ShardGenerator;
    private readonly IMerchantFactory MerchantFactory;

    private Point CenterWarpPlayer;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    
    /// <inheritdoc />
    public ArenaUndergroundScript(
        Dialog subject,
        IShardGenerator shardGenerator,
        ISimpleCache simpleCache,
        IClientRegistry<IWorldClient> clientRegistry,
        IMerchantFactory merchantFactory
    )
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        ClientRegistry = clientRegistry;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
        ShardGenerator = shardGenerator;
    }

    public void HideDialogOptions(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ArenaHost stage);

        if ((stage != ArenaHost.Host) && (stage != ArenaHost.MasterHost))
            RemoveOption(Subject, "Host Options");
    }
    
    private Point GetValidRandomPoint(IRectangle rect)
    {
        Point randomPoint;
        
        randomPoint = rect.GetRandomPoint();

        return randomPoint;
    }
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_initial":
                HideDialogOptions(source);

                break;

            case "ophie_golddefending":
            {
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
                    aisling.Trackers.Enums.TryGetValue(out ArenaSide side);

                    if (team is ArenaTeam.Blue or ArenaTeam.Red)
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage("All players should be on Green or Gold team.");
                        return;
                    }
                    if (team == ArenaTeam.Gold)
                    {
                        aisling.Trackers.Enums.Set(ArenaSide.Defender);
                    }

                    if (team == ArenaTeam.Green)
                    {
                        aisling.Trackers.Enums.Set(ArenaSide.Offensive);
                    }
                }

                break;
            }
            
            case "ophie_greendefending":
            {
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
                    aisling.Trackers.Enums.TryGetValue(out ArenaSide side);

                    if (team is ArenaTeam.Blue or ArenaTeam.Red)
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage("All players should be on Green or Gold team.");
                        return;
                    }
                    
                    if (team == ArenaTeam.Gold)
                    {
                        aisling.Trackers.Enums.Set(ArenaSide.Offensive);
                    }

                    if (team == ArenaTeam.Green)
                    {
                        aisling.Trackers.Enums.Set(ArenaSide.Defender);
                    }
                }

                break;
            }
            
            case "ophie_startescort":
            {
                var shard = ShardGenerator.CreateShardOfInstance("arena_escort");
                shard.Shards.TryAdd(shard.InstanceId, shard);
                var script = shard.Script.As<EscortScript>();


                if (script == null)
                    shard.AddScript<EscortScript>();
                
                var nathra = MerchantFactory.Create("nathra", shard, new Point(5, 37));
                shard.AddEntity(nathra, nathra);
                
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
                    aisling.Trackers.Enums.TryGetValue(out ArenaSide side);
                    
                    
                    Rectangle targetRectangle;

                    if (side == ArenaSide.Offensive)
                    {
                        targetRectangle = new Rectangle(new Point(35, 6), 2, 2);
                        aisling.TraverseMap(shard, GetValidRandomPoint(targetRectangle));
                    }
                    if (side == ArenaSide.Defender)
                    {
                        targetRectangle = new Rectangle(new Point(6, 30), 3, 3);
                        aisling.TraverseMap(shard, GetValidRandomPoint(targetRectangle));
                        break;
                    }
                }

                Subject.Close(source);
                break;
                
            }
            
            case "ophie_startcolorclash":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_colorclash");
                var script = mapInstance.Script.As<ColorClashScript>();

                if (script == null)
                    mapInstance.AddScript<ColorClashScript>();

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
                            aisling.TraverseMap(mapInstance, ColorClashBluePoint);

                            break;
                        case ArenaTeam.Green:
                            aisling.TraverseMap(mapInstance, ColorClashGreenPoint);

                            break;
                        case ArenaTeam.Gold:
                            aisling.TraverseMap(mapInstance, ColorClashGoldPoint);

                            break;
                        case ArenaTeam.Red:
                            aisling.TraverseMap(mapInstance, ColorClashRedPoint);

                            break;
                    }
                }

                Subject.Close(source);
                break;
            }
            
            case "ophie_everyonewon":
            {
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, "You have won the arena event!");

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

                    var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                    var thirtyPercent = Convert.ToInt32(.30 * tnl);
                
                    ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    
                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
                    while (CenterWarp == CenterWarpPlayer);

                    aisling.Trackers.Enums.Remove<ArenaTeam>();
                    aisling.Trackers.Enums.Remove<ArenaSide>();
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
                            await channel.SendMessageAsync($"{aisling.Name} won!");

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
                                await channel.SendMessageAsync($"{aisling.Name} won!");
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
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team has won the arena event!");

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
                        
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var thirtyPercent = Convert.ToInt32(.30 * tnl);
                        ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team lost. Better luck next time!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "uundrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var tenPercent = Convert.ToInt32(.10 * tnl);
                        ExperienceDistributionScript.GiveExp(source, tenPercent);
                    }
                    
                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
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
                                await channel.SendMessageAsync($"{aisling.Name} won!");
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
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team has won the arena event!");

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
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var thirtyPercent = Convert.ToInt32(.30 * tnl);
                        ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team lost. Better luck next time!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdWin",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var tenPercent = Convert.ToInt32(.10 * tnl);
                        ExperienceDistributionScript.GiveExp(source, tenPercent);
                    }

                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
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
                                await channel.SendMessageAsync($"{aisling.Name} won!");
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
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team has won the arena event!");

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
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var thirtyPercent = Convert.ToInt32(.30 * tnl);
                        ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team lost. Better luck next time!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var tenPercent = Convert.ToInt32(.30 * tnl);
                        ExperienceDistributionScript.GiveExp(source, tenPercent);
                    }

                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
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
                                await channel.SendMessageAsync($"{aisling.Name} won!");
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
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team has won the arena event!");

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
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var thirtyPercent = Convert.ToInt32(.30 * tnl);
                        ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    }
                    else
                    {
                        aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team lost. Better luck next time!");

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Arena Underground Participant",
                                "undrGrdPart",
                                MarkIcon.Victory,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                        
                        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
                        var tenPercent = Convert.ToInt32(.30 * tnl);
                        ExperienceDistributionScript.GiveExp(source, tenPercent);
                    }

                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
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
                    aisling.SendServerMessage(
                        ServerMessageType.OrangeBar2,
                        $"Arena Host {source.Name} is announcing a Skandara's Gauntlet!");

                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync(
                            $"Arena Host {source.Name
                            } is announcing a Skandara's Gauntlet! Please head to the Arena Underground to participate.");

                        await client.StopAsync();
                    });

                break;
            }

            case "ophie_serendaelsandbox":
            {
                foreach (var aisling in ClientRegistry)
                    aisling.SendServerMessage(
                        ServerMessageType.OrangeBar2,
                        $"Arena Host {source.Name} is announcing a Serendael's Sandbox!");

                Task.Run(
                    async () =>
                    {
                        var client = new DiscordSocketClient();
                        await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                        await client.StartAsync();
                        var channel = await client.GetChannelAsync(CHANNEL_ID) as IMessageChannel;

                        await channel!.SendMessageAsync(
                            $"Arena Host {source.Name
                            } is announcing a Serendael's Sandbox! Please head to the Arena Underground to participate.");

                        await client.StopAsync();
                    });

                break;
            }
            
            case "ophie_starthiddenhavochostnotplayingstart":
            {
                source.Trackers.Enums.Set(ArenaHostPlaying.No);

                var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
                shard.Shards.TryAdd(shard.InstanceId, shard);

                   
                var script = shard.Script.As<HiddenHavocShrinkScript>();
                
                if (script == null)
                   shard.AddScript<HiddenHavocShrinkScript>();

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;

                    do
                        point = shard.Template.Bounds.GetRandomPoint();
                    while (shard.IsWall(point) || shard.IsBlockingReactor(point));

                    aisling.TraverseMap(shard, point);
                }

                Subject.Close(source);
                break;
            }

            case "ophie_starthiddenhavochostplayingstart":
            {
                source.Trackers.Enums.Set(ArenaHostPlaying.Yes);

                var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
                shard.Shards.TryAdd(shard.InstanceId, shard);
                var script = shard.Script.As<HiddenHavocShrinkScript>();
                
                if (script == null)
                    shard.AddScript<HiddenHavocShrinkScript>();

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;

                    do
                        point = shard.Template.Bounds.GetRandomPoint();
                    while (shard.IsWall(point) || shard.IsBlockingReactor(point));

                    aisling.TraverseMap(shard, point);
                }

                Subject.Close(source);

                break;
            }

            case "ophie_startffalavaflowhostnotplayingstart":
            {
                source.Trackers.Enums.Set(ArenaHostPlaying.No);

                var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
                shard.Shards.TryAdd(shard.InstanceId, shard);
                var script = shard.Script.As<LavaFlowShrinkScript>();

                if (script == null)
                    shard.AddScript<LavaFlowShrinkScript>();

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;

                    do
                        point = shard.Template.Bounds.GetRandomPoint();
                    while (shard.IsWall(point) || shard.IsBlockingReactor(point));

                    aisling.TraverseMap(shard, point);
                }

                Subject.Close(source);

                break;
            }

            case "ophie_startffalavaflowhostplayingstart":
            {
                source.Trackers.Enums.Set(ArenaHostPlaying.Yes);

                var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
                shard.Shards.TryAdd(shard.InstanceId, shard);
                var script = shard.Script.As<LavaFlowShrinkScript>();

                if (script == null)
                    shard.AddScript<LavaFlowShrinkScript>();

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;

                    do
                        point = shard.Template.Bounds.GetRandomPoint();
                    while (shard.IsWall(point) || shard.IsBlockingReactor(point));

                    aisling.TraverseMap(shard, point);
                }

                Subject.Close(source);

                break;
            }

            case "ophie_startteamgamelavaflowhostplayingstart":
            {
                source.Trackers.Enums.Set(ArenaHostPlaying.Yes);

                var shard = ShardGenerator.CreateShardOfInstance("arena_lavateams");
                shard.Shards.TryAdd(shard.InstanceId, shard);
                var script = shard.Script.As<LavaFlowShrinkScript>();

                if (script == null)
                    shard.AddScript<LavaFlowShrinkScript>();

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
                            aisling.TraverseMap(shard, LavaBluePoint);
                            break;
                        case ArenaTeam.Green:
                            aisling.TraverseMap(shard, LavaGreenPoint);
                            break;
                        case ArenaTeam.Gold:
                            aisling.TraverseMap(shard, LavaGoldPoint);
                            break;
                        case ArenaTeam.Red:
                            aisling.TraverseMap(shard, LavaRedPoint);
                            break;
                    }
                }

                Subject.Close(source);

                break;
            }

            case "ophie_startteamgamelavaflowhostnotplayingstart":
            {
                    source.Trackers.Enums.Set(ArenaHostPlaying.No);

                    var shard = ShardGenerator.CreateShardOfInstance("arena_lavateams");
                    shard.Shards.TryAdd(shard.InstanceId, shard);
                    var script = shard.Script.As<LavaFlowShrinkScript>();

                if (script == null)
                    shard.AddScript<LavaFlowShrinkScript>();

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
                                aisling.TraverseMap(shard, LavaBluePoint);
                                break;
                            case ArenaTeam.Green:
                                aisling.TraverseMap(shard, LavaGreenPoint);
                                break;
                            case ArenaTeam.Gold:
                                aisling.TraverseMap(shard, LavaGoldPoint);
                                break;
                            case ArenaTeam.Red:
                                aisling.TraverseMap(shard, LavaRedPoint);
                                break;
                        }
                    }

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

                source.MapInstance.RemoveEntity(source);
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
                    CenterWarpPlayer = rect.GetRandomPoint();
                while (CenterWarp == CenterWarpPlayer);

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Remove<ArenaTeam>();
                aisling?.Trackers.Enums.Remove<ArenaSide>();
                aisling?.WarpTo(CenterWarpPlayer);

                break;
            }
        }
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