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
    private const string BOT_TOKEN = @"MTA4Mzg2MzMyNDc3MDQzOTM1MA.Gek2EZ.se-vh-fRnr78vAYuub5KUK-aIzrUHEMlEkB75Y";
    private const ulong CHANNEL_ID = 1136412469762470038;
    private const ulong ARENA_WIN_CHANNEL_ID = 1136426304300916786;
    private readonly Point CenterWarp = new(11, 10);
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly Point LavaBluePoint = new(8, 23);
    private readonly Point LavaGoldPoint = new(23, 21);
    private readonly Point LavaGreenPoint = new(23, 6);
    private readonly Point LavaRedPoint = new(8, 5);
    private readonly Point ColorClashBluePoint = new(27, 27);
    private readonly Point ColorClashGoldPoint = new(4, 4);
    private readonly Point ColorClashGreenPoint = new(26, 6);
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
        IClientRegistry<IChaosWorldClient> clientRegistry,
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

    private void HandleSpecialEvents(Aisling source, string key)
    {
        switch (key)
        {
            case "ophie_battlering":
                EnterBattleRing(source);
                break;
            case "ophie_revive":
                ReviveAisling(source);
                break;
            case "ophie_leave":
                LeaveArena(source);
                break;
            case "ophie_skandaragauntlet":
                AnnouceToWorld($"Host {source.Name} is announcing a Skandara's Gauntlet event!");
                break;
            case "ophie_serendaelsandbox":
                AnnouceToWorld($"Host {source.Name} is announcing a Serendael Sandbox event!");
                break;
            default:
                source.SendServerMessage(ServerMessageType.OrangeBar2, "Unrecognized event command.");
                break;
        }
    }

    public override void OnDisplaying(Aisling source)
    {
        var key = Subject.Template.TemplateKey.ToLower();

        switch (key)
        {
            case "ophie_initial":
                HideDialogOptions(source);
                break;
            
            case "ophie_golddefending":
                SetDefendingTeams(source, ArenaTeam.Gold);
                break;
            case "ophie_greendefending":
                SetDefendingTeams(source, ArenaTeam.Green);
                break;
            
            case "ophie_startescort":
                StartEscortMission(source);
                break;
            
            case "ophie_starthuntinggroundsstart":
                StartHuntingGrounds(source);
                break;
            
            case "ophie_startcolorclash":
                StartColorClash(source);
                break;
            
            case "ophie_starthiddenhavochostnotplayingstart":
                StartHiddenHavoc(source, false);
                break;
            
            case "ophie_starthiddenhavochostplayingstart":
                StartHiddenHavoc(source, true);
                break;
            
            case "ophie_startffalavaflowhostnotplayingstart":
                StartLavaFlow(source, false);
                break;
            
            case "ophie_startffalavaflowhostplayingstart":
                StartLavaFlow(source, true);
                break;
            
            case "ophie_startteamgamelavaflowhostnotplayingstart":
                StartTeamGameLavaFlow(source, false);
                break;
            
            case "ophie_startteamgamelavaflowhostplayingstart":
                StartTeamGameLavaFlow(source, true);
                break;
            
            case "ophie_everyonewon":
                AwardEveryone(source);
                break;
            
            case "ophie_goldwon":
            case "ophie_bluewon":
            case "ophie_greenwon":
            case "ophie_redwon":
                AwardTeamWin(source, key);
                break;
            
            case "ophie_battlering":
            case "ophie_revive":
            case "ophie_leave":
            case "ophie_skandaragauntlet":
            case "ophie_serendaelsandbox":
                HandleSpecialEvents(source, key);
                break;
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
                
                if (aisling!.MapInstance.InstanceId != "arena_underground")
                    return;
                
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
                
                if (aisling!.MapInstance.InstanceId != "arena_underground")
                    return;
                
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
                
                if (aisling!.MapInstance.InstanceId != "arena_underground")
                    return;
                
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

                if (aisling!.MapInstance.InstanceId != "arena_underground")
                    return;
                
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
                
                if (aisling!.MapInstance.InstanceId != "arena_underground")
                    return;
                
                aisling?.Trackers.Enums.Remove<ArenaTeam>();
                aisling?.Trackers.Enums.Remove<ArenaSide>();
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
    
    private void SetDefendingTeams(Aisling source, ArenaTeam defendingTeam)
    {
        if (defendingTeam is ArenaTeam.Gold)
        {
            foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
            {
                aisling.Trackers.Enums.Remove<ArenaSide>();
                
                aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

                if (team is ArenaTeam.Gold)
                    aisling.Trackers.Enums.Set(ArenaSide.Defender);
                else
                    aisling.Trackers.Enums.Set(ArenaSide.Offensive);
            }
        }
        if (defendingTeam is ArenaTeam.Green)
        {
            foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
            {
                aisling.Trackers.Enums.Remove<ArenaSide>();
                aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

                if (team is ArenaTeam.Green)
                    aisling.Trackers.Enums.Set(ArenaSide.Defender);
                else
                    aisling.Trackers.Enums.Set(ArenaSide.Offensive);
            }
        }
    }

    private void StartHuntingGrounds(Aisling source)
    {
        var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        var script = shard.Script.As<HuntingGroundsScript>();
                
        if (script == null)
            shard.AddScript<HuntingGroundsScript>();

        TeleportParticipants(source, shard, shard.Template.Bounds);
        Subject.Close(source);
    }
    
    private void StartEscortMission(Aisling source)
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
                    
            aisling.Trackers.Enums.TryGetValue(out ArenaSide side);

            if (side == ArenaSide.Offensive)
                aisling.TraverseMap(shard, new Point(5, 35));

            if (side == ArenaSide.Defender)
                aisling.TraverseMap(shard, new Point(39, 6));
        }

        Subject.Close(source);
    }
    
    private void StartColorClash(Aisling source)
    {
        var shard = ShardGenerator.CreateShardOfInstance("arena_colorclash");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        var script = shard.Script.As<ColorClashScript>();
        
        if (script == null)
            shard.AddScript<ColorClashScript>();
        

        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            if (!aisling.IsAlive)
            {
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(100);
                aisling.StatSheet.SetManaPct(100);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                source.Display();
            }

            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

            switch (team)
            {
                case ArenaTeam.Blue:
                    aisling.TraverseMap(shard, ColorClashBluePoint);

                    break;
                case ArenaTeam.Green:
                    aisling.TraverseMap(shard, ColorClashGreenPoint);

                    break;
                case ArenaTeam.Gold:
                    aisling.TraverseMap(shard, ColorClashGoldPoint);

                    break;
                case ArenaTeam.Red:
                    aisling.TraverseMap(shard, ColorClashRedPoint);

                    break;
                case ArenaTeam.None:
                    aisling.SendActiveMessage("The host didn't give you a team!");
                    break;
                
                default:
                    aisling.SendActiveMessage("The host didn't give you a team!");
                    break;
            }
        }
    }

    private void SendMessageToDiscord(string message) =>
        Task.Run(async () =>
        {
            await using var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
            await client.StartAsync();
            var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;
            await channel!.SendMessageAsync(message);
            await client.StopAsync();
        });

    private void AwardEveryone(Aisling source)
    {
        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            aisling.SendServerMessage(ServerMessageType.OrangeBar2, "You have won the arena event!");

            GiveRewards(aisling, 1.0);

            var rect = new Rectangle(new Point(11, 10), 3, 4);

            do
                CenterWarpPlayer = rect.GetRandomPoint();
            while (CenterWarp == CenterWarpPlayer);

            aisling.Trackers.Enums.Remove<ArenaTeam>();
            aisling.Trackers.Enums.Remove<ArenaSide>();
            aisling.WarpTo(CenterWarpPlayer);
        }

        var merchant = Subject.DialogSource as Merchant;
        merchant?.Say("Win is cast! Congrats everyone!");
        
        SendMessageToDiscord($"Arena Host {source.Name} cast win for all in attendance!");
        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
            SendMessageToDiscord($"{aisling.Name} won!");
    }

    private void AwardTeamWin(Aisling source, string teamKey)
    {
        var teamName = teamKey.Replace("ophie_", "").Replace("won", "");
        var winningTeam = Enum.Parse<ArenaTeam>(teamName, true);
        
        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
        
            if (team == winningTeam)
            {
                aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team has won the arena event!");
                GiveRewards(aisling, 1.0);
            }
            else
            {
                aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team did not win. Better luck next time!");
                GiveRewards(aisling, 0.5);
            }
            
            var rect = new Rectangle(new Point(11, 10), 3, 4);

            do
                CenterWarpPlayer = rect.GetRandomPoint();
            while (CenterWarp == CenterWarpPlayer);

            aisling.Trackers.Enums.Remove<ArenaTeam>();
            aisling.Trackers.Enums.Remove<ArenaSide>();
            aisling.WarpTo(CenterWarpPlayer);
        }
        
        var merchant = Subject.DialogSource as Merchant;
        merchant?.Say($"Win is cast! Congrats {winningTeam}!");
        SendMessageToDiscord($"{winningTeam} team has won the event!");
    }
    
    private void GiveRewards(Aisling aisling, double rewardMultiplier)
    {
        var baseExperience = CalculateBaseExperience(aisling);
        var experienceAward = (int)(baseExperience * rewardMultiplier);
        ExperienceDistributionScript.GiveExp(aisling, experienceAward);
        
        aisling.TryGiveGold((int)(1000 * rewardMultiplier));
        
        aisling.Legend.AddOrAccumulate(
            new LegendMark(
                "Arena Participant",
                "arenaParticipation",
                MarkIcon.Victory,
                MarkColor.White,
                1,
                GameTime.Now
            )
        );

        if (rewardMultiplier is 1.0)
            aisling.Legend.AddOrAccumulate(
                new LegendMark(
                    "Arena Winner",
                    "arenaWinner",
                    MarkIcon.Victory,
                    MarkColor.Blue,
                    1,
                    GameTime.Now
                )
            );
    }

    private int CalculateBaseExperience(Aisling aisling)
    {
        var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
        var thirtyPercent = Convert.ToInt32(.30 * tnl);

        if (aisling.UserStatSheet.Level == 99)
        {
            thirtyPercent = 15000000;
        }
        
        return thirtyPercent;
    }
    
    private Point GetValidRandomPoint(IRectangle rect)
    {
        var randomPoint = rect.GetRandomPoint();

        return randomPoint;
    }
    
    private void AnnouceToWorld(string message)
    {
        foreach (var aisling in ClientRegistry)
            aisling.SendServerMessage(
                ServerMessageType.OrangeBar2,
                message);
    }
    private void EnterBattleRing(Aisling source)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
        source.TraverseMap(mapInstance, new Point(13, 13));
        source.SendServerMessage(ServerMessageType.OrangeBar2, "You have entered the battle ring!");
        Subject.Close(source);
    }

    private void ReviveAisling(Aisling source)
    {
        if (!source.IsAlive)
        {
            source.IsDead = false;
            source.StatSheet.SetHealthPct(100);
            source.StatSheet.SetManaPct(100);
            source.Client.SendAttributes(StatUpdateType.Vitality);
            source.SendActiveMessage("Ophie has revived you.");
            source.Refresh();
            source.Display();

            source.SendServerMessage(ServerMessageType.OrangeBar2, "You have been revived by Ophie.");
        }
        else
            source.SendServerMessage(ServerMessageType.OrangeBar2, "No need for revival, you are already alive.");

        Subject.Close(source);
    }

    private void StartHiddenHavoc(Aisling source, bool hostPlaying)
    {
        source.Trackers.Enums.Set(hostPlaying ? ArenaHostPlaying.Yes : ArenaHostPlaying.No);
        var shard = ShardGenerator.CreateShardOfInstance("arena_hidden_havoc");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        var script = shard.Script.As<HiddenHavocShrinkScript>();
                
        if (script == null)
            shard.AddScript<HiddenHavocShrinkScript>();

        TeleportParticipants(source, shard, shard.Template.Bounds);
        Subject.Close(source);
    }

    private void StartLavaFlow(Aisling source, bool hostPlaying)
    {
        source.Trackers.Enums.Set(hostPlaying ? ArenaHostPlaying.Yes : ArenaHostPlaying.No);
        var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        var script = shard.Script.As<LavaFlowShrinkScript>();
                
        if (script == null)
            shard.AddScript<LavaFlowShrinkScript>();

        TeleportParticipants(source, shard, shard.Template.Bounds);
        Subject.Close(source);
    }

    private void StartTeamGameLavaFlow(Aisling source, bool hostPlaying)
    {
        source.Trackers.Enums.Set(hostPlaying ? ArenaHostPlaying.Yes : ArenaHostPlaying.No);
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
                aisling.Refresh();
            }

            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
            TeleportToTeamStartingPoint(aisling, shard, team);
        }

        Subject.Close(source);
    }

    private void TeleportParticipants(Aisling source, MapInstance shard, IRectangle bounds)
    {
        var teleportPoints = new List<Point>();

        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            Point point;
            bool isInvalidPoint;

            do
            {
                point = bounds.GetRandomPoint();
                isInvalidPoint = shard.IsWall(point) || shard.IsBlockingReactor(point) || IsPointTooCloseToOthers(point, teleportPoints);
            }
            while (isInvalidPoint);

            teleportPoints.Add(point);
            aisling.TraverseMap(shard, point);
        }
    }

    private bool IsPointTooCloseToOthers(Point point, List<Point> points, int minDistance = 1)
    {
        foreach (var existingPoint in points)
            if ((Math.Abs(existingPoint.X - point.X) <= minDistance) && (Math.Abs(existingPoint.Y - point.Y) <= minDistance))
                return true;

        return false;
    }

    
    
    private void TeleportToTeamStartingPoint(Aisling aisling, MapInstance shard, ArenaTeam team)
    {
        var startingPoint = team switch
        {
            ArenaTeam.Blue  => LavaBluePoint,
            ArenaTeam.Green => LavaGreenPoint,
            ArenaTeam.Gold  => LavaGoldPoint,
            ArenaTeam.Red   => LavaRedPoint,
            _               => shard.Template.Bounds.GetRandomPoint()
        };

        aisling.TraverseMap(shard, startingPoint);
    }

    private void LeaveArena(Aisling source)
    {
        Subject.Close(source);
        var worldMap = SimpleCache.Get<WorldMap>("field001");

        //if we cant set the active object, return
        if (!source.ActiveObject.SetIfNull(worldMap))
            return;

        if (source.IsDead)
        {
            source.IsDead = false;
            source.StatSheet.SetHealthPct(25);
            source.StatSheet.SetManaPct(25);
            source.Client.SendAttributes(StatUpdateType.Vitality);
            source.SendActiveMessage("You have been revived.");
            source.Refresh();
            source.Display();
        }

        source.Trackers.Enums.Remove<ArenaTeam>();
        source.MapInstance.RemoveEntity(source);
        source.Client.SendWorldMap(worldMap);
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