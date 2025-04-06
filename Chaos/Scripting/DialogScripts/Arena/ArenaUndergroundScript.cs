using Chaos.Collections;
using Chaos.DarkAges.Definitions;
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
using Chaos.Scripting.MapScripts.Arena.Arena_Modules;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaUndergroundScript : DialogScriptBase
{
    private const string BOT_TOKEN = @"";
    private const ulong CHANNEL_ID = 1136412469762470038;
    private const ulong ARENA_WIN_CHANNEL_ID = 1136426304300916786;
    
    private static bool FreeForAll;
    private readonly Point CenterWarp = new(11, 10);
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly Point ColorClashBluePoint = new(27, 27);
    private readonly Point ColorClashCenter = new(15, 15);
    private readonly Point ColorClashGoldPoint = new(4, 4);
    private readonly Point ColorClashGreenPoint = new(27, 4);
    private readonly Point ColorClashRedPoint = new(4, 27);
    private readonly Point LavaBluePoint = new(8, 23);
    private readonly Point LavaGoldPoint = new(23, 21);
    private readonly Point LavaGreenPoint = new(23, 6);
    private readonly Point LavaRedPoint = new(8, 5);
    private readonly IMerchantFactory MerchantFactory;
    private readonly IShardGenerator ShardGenerator;
    private readonly ISimpleCache SimpleCache;

    private Point CenterWarpPlayer;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public ArenaUndergroundScript(
        Dialog subject,
        IShardGenerator shardGenerator,
        ISimpleCache simpleCache,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IMerchantFactory merchantFactory)
        : base(subject)
    {
        FreeForAll = false;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        ClientRegistry = clientRegistry;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
        ShardGenerator = shardGenerator;
    }

    private void AnnouceToWorld(string message, Aisling source)
    {
        foreach (var aisling in ClientRegistry)
            aisling.SendServerMessage(ServerMessageType.OrangeBar2, message);
        
        if (source.IsArenaHost()) 
            source.Trackers.Enums.Set(HostingArena.Yes);
    }

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
        var teamName = teamKey.Replace("ophie_", "")
                              .Replace("won", "");
        var winningTeam = Enum.Parse<ArenaTeam>(teamName, true);

        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

            if (team == winningTeam)
            {
                aisling.SendServerMessage(ServerMessageType.OrangeBar2, "Your team has won the arena event!");
                GiveRewards(aisling, 1.0);
            } else
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

    private static int CalculateBaseExperience(Aisling aisling)
    {
        const int MAX_LEVEL = 99;
        const int MAX_LEVEL_EXPERIENCE = 15000000;

        return aisling.UserStatSheet.Level == MAX_LEVEL
            ? MAX_LEVEL_EXPERIENCE
            : Convert.ToInt32(0.30 * LevelUpFormulae.Default.CalculateTnl(aisling));
    }

    private void EnterBattleRing(Aisling source)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
        source.TraverseMap(mapInstance, new Point(13, 13));
        source.SendServerMessage(ServerMessageType.OrangeBar2, "You have entered the battle ring!");
        Subject.Close(source);
    }

    private static Point GetValidRandomPoint(IRectangle rect)
    {
        var randomPoint = rect.GetRandomPoint();

        return randomPoint;
    }

    private static Point GetValidTeleportPoint(MapInstance shard, IRectangle bounds, List<Point> teleportPoints)
    {
        Point point;

        do
            point = bounds.GetRandomPoint();
        while (shard.IsWall(point) || shard.IsBlockingReactor(point) || IsPointTooCloseToOthers(point, teleportPoints));

        return point;
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
                GameTime.Now));

        if (rewardMultiplier is 1.0)
            aisling.Legend.AddOrAccumulate(
                new LegendMark(
                    "Arena Winner",
                    "arenaWinner",
                    MarkIcon.Victory,
                    MarkColor.Blue,
                    1,
                    GameTime.Now));
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
            case "ophie_allmodes":
                AnnouceToWorld($"{{=a{source.Name} is hosting {{=bPvP {{=aand {{=qPvE {{=amodes in Drowned Labyrinth.", source);

                break;
            case "ophie_pvpmodes":
                AnnouceToWorld($"{{=a{source.Name} is hosting {{=bPvP {{=aonly modes in Drowned Labyrinth.", source);

                break;
            
            case "ophie_pvemodes":
                AnnouceToWorld($"{{=a{source.Name} is hosting {{=qPvE {{=aonly modes in Drowned Labyrinth.", source);

                break;

            default:
                source.SendServerMessage(ServerMessageType.OrangeBar2, "Unrecognized event command.");

                break;
        }
    }

    private void HideDialogOptions(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ArenaHost stage);

        if ((stage != ArenaHost.Host) && (stage != ArenaHost.MasterHost))
            RemoveOption(Subject, "Host Options");
    }

    private static bool IsPointTooCloseToOthers(Point point, List<Point> points, int minDistance = 3)
        => points.Any(existingPoint =>
            Math.Abs(existingPoint.X - point.X) + Math.Abs(existingPoint.Y - point.Y) < minDistance);
    
    private void LeaveArena(Aisling source)
    {
        Subject.Close(source);
        var worldMap = SimpleCache.Get<WorldMap>("field001");

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

        if (source.IsArenaHost()) 
            source.Trackers.Enums.Set(HostingArena.None);
        
        source.Trackers.Enums.Remove<ArenaTeam>();
        source.MapInstance.RemoveEntity(source);
        source.Client.SendWorldMap(worldMap);
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
                StartColorClash(source, false);

                break;

            case "ophie_starttypingarena":
                StartTypingArena(source);

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
            case "ophie_pvpmodes":
            case "ophie_pvemodes":
            case "ophie_allmodes":
                HandleSpecialEvents(source, key);

                break;
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace)
            && !Subject.Template.TemplateKey.Equals("ophie_initial", StringComparison.CurrentCultureIgnoreCase))
            return;

        var aisling = source.MapInstance
                            .GetEntities<Aisling>()
                            .FirstOrDefault(x => (playerToPlace != null) && x.Name.EqualsI(playerToPlace));

        if ((aisling == null) || (aisling.MapInstance.InstanceId != "arena_underground"))
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_placeonredconfirm":
                PlacePlayerOnTeam(
                    aisling,
                    ArenaTeam.Red,
                    new Rectangle(new Point(11, 17), 4, 3),
                    "Red");

                break;

            case "ophie_placeongreenconfirm":
                PlacePlayerOnTeam(
                    aisling,
                    ArenaTeam.Green,
                    new Rectangle(new Point(18, 10), 3, 4),
                    "Green");

                break;

            case "ophie_placeongoldconfirm":
                PlacePlayerOnTeam(
                    aisling,
                    ArenaTeam.Gold,
                    new Rectangle(new Point(5, 10), 4, 3),
                    "Gold");

                break;

            case "ophie_placeonblueconfirm":
                PlacePlayerOnTeam(
                    aisling,
                    ArenaTeam.Blue,
                    new Rectangle(new Point(12, 4), 4, 3),
                    "Blue");

                break;

            case "ophie_removeteamconfirm":
                RemovePlayerFromTeam(aisling, new Rectangle(new Point(11, 10), 3, 4));

                break;
        }
    }

    private static void PlacePlayerOnTeam(
        Aisling aisling,
        ArenaTeam team,
        Rectangle rect,
        string teamColor)
    {
        aisling.Trackers.Enums.Set(team);
        aisling.WarpTo(rect.GetRandomPoint());
        aisling.SendActiveMessage($"You've been placed on the {teamColor} team!");
    }

    private static void RemoveOption(Dialog subject, string optionName)
    {
        var optionIndex = subject.GetOptionIndex(optionName);

        if (optionIndex.HasValue)
            subject.Options.RemoveAt(optionIndex.Value);
    }

    private void RemovePlayerFromTeam(Aisling aisling, Rectangle rect)
    {
        Point centerWarpPlayer;

        do
            centerWarpPlayer = rect.GetRandomPoint();
        while (centerWarpPlayer == CenterWarp);

        aisling.Trackers.Enums.Remove<ArenaTeam>();
        aisling.Trackers.Enums.Remove<ArenaSide>();
        aisling.WarpTo(centerWarpPlayer);
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
        } else
            source.SendServerMessage(ServerMessageType.OrangeBar2, "No need for revival, you are already alive.");

        Subject.Close(source);
    }

    private static void SendMessageToDiscord(string message)
        => Task.Run(
            async () =>
            {
                await using var client = new DiscordSocketClient();
                await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                await client.StartAsync();
                var channel = await client.GetChannelAsync(ARENA_WIN_CHANNEL_ID) as IMessageChannel;
                await channel!.SendMessageAsync(message);
                await client.StopAsync();
            });

    private static void SetDefendingTeams(Aisling source, ArenaTeam defendingTeam)
    {
        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            aisling.Trackers.Enums.Remove<ArenaSide>();
            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

            aisling.Trackers.Enums.Set(team == defendingTeam ? ArenaSide.Defender : ArenaSide.Offensive);
        }
    }

    private void StartColorClash(Aisling source, bool hostPlaying)
    {
        source.Trackers.Enums.Set(hostPlaying ? ArenaHostPlaying.Yes : ArenaHostPlaying.No);
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
                    aisling.TraverseMap(shard, ColorClashCenter);

                    break;

                default:
                    aisling.SendActiveMessage("The host didn't give you a team!");
                    aisling.TraverseMap(shard, ColorClashCenter);

                    break;
            }
        }
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

    private void StartHiddenHavoc(Aisling source, bool hostPlaying)
    {
        source.Trackers.Enums.Set(hostPlaying ? ArenaHostPlaying.Yes : ArenaHostPlaying.No);
        var shard = ShardGenerator.CreateShardOfInstance("arena_hidden_havoc");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        FreeForAll = true;
        var script = shard.Script.As<HiddenHavocShrinkScript>();

        if (script == null)
            shard.AddScript<HiddenHavocShrinkScript>();

        TeleportParticipants(source, shard, shard.Template.Bounds);
        Subject.Close(source);
    }

    private void StartHuntingGrounds(Aisling source)
    {
        var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        var script = shard.Script.As<HuntingGroundsScript>();
        FreeForAll = true;

        if (script == null)
            shard.AddScript<HuntingGroundsScript>();

        TeleportParticipants(source, shard, shard.Template.Bounds);
        Subject.Close(source);
    }

    private void StartLavaFlow(Aisling source, bool hostPlaying)
    {
        source.Trackers.Enums.Set(hostPlaying ? ArenaHostPlaying.Yes : ArenaHostPlaying.No);
        var shard = ShardGenerator.CreateShardOfInstance("arena_lava");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        FreeForAll = true;
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

    private void StartTypingArena(Aisling source)
    {
        var shard = ShardGenerator.CreateShardOfInstance("arena_typing");
        shard.Shards.TryAdd(shard.InstanceId, shard);
        FreeForAll = true;
        var script = shard.Script.As<TypingArenaMapScript>();

        if (script == null)
            shard.AddScript<TypingArenaMapScript>();

        TeleportParticipants(source, shard, shard.Template.Bounds);
        Subject.Close(source);
    }

    private static void TeleportParticipants(Aisling source, MapInstance shard, IRectangle bounds)
    {
        var teleportPoints = new List<Point>();

        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
        {
            if (FreeForAll && !aisling.IsGodModeEnabled())
                if (aisling.Group?.Count > 1)
                    aisling.Group.Kick(aisling);

            var point = GetValidTeleportPoint(shard, bounds, teleportPoints);
            teleportPoints.Add(point);
            aisling.TraverseMap(shard, point);
        }
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
}