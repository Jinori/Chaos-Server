using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Events.Easter;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MapScripts.Events.Easter;

public sealed partial class HopocalypseScript : MapScriptBase
{
    private readonly IStorage<HopocalypseLeaderboardScript.HopocalypseLeaderboardObj> LeaderboardStorage;
    private bool AnnounceStart;
    private bool SpawnedBunnies;
    private bool GameEnded;
    private bool PlayStartSound;
    private int GameLevel;
    private readonly IIntervalTimer MessageTimer;
    private readonly IIntervalTimer CheckTouchTimer;
    private readonly IIntervalTimer CheckEggTimer;
    private readonly Point PlayerStartPoint = new(10, 14);
    private readonly IMonsterFactory MonsterFactory;
    private readonly IItemFactory ItemFactory;
    private readonly List<string> BunnyNames = ["Burrowglint", "Hopscare", "Petalpounce", "Whiskerflip"];
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public HopocalypseScript(MapInstance subject, IMonsterFactory monsterFactory, IItemFactory itemFactory, ISimpleCache simpleCache, IStorage<HopocalypseLeaderboardScript.HopocalypseLeaderboardObj> leaderboardStorage)
        : base(subject)
    {
        MessageTimer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(7),
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(1),
            "Get ready to collect eggs in {Time}!",
            SendMessage);

        CheckTouchTimer = new IntervalTimer(TimeSpan.FromMilliseconds(350));
        CheckEggTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        LeaderboardStorage = leaderboardStorage;
        MonsterFactory = monsterFactory;
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }


    private void SendMessage(string message)
    {
        foreach (var player in Subject.GetEntities<Aisling>())
            player.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public override void OnMorphed() => Subject.Pathfinder.RegisterGrid(Subject);

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        MessageTimer.Update(delta);
        CheckTouchTimer.Update(delta);
        CheckEggTimer.Update(delta);


        if (CheckEggTimer.IntervalElapsed && !GameEnded && AnnounceStart)
        {
            var eggs = Subject.GetEntities<GroundItem>()
                              .Where(x => x.Item.Template.TemplateKey is "undinechickenegg" or "undinegoldenchickenegg").ToList();

            if (eggs.Count is 0)
            {
                AnnounceStart = false;
                SpawnedBunnies = false;
                PlayStartSound = false;
                GameEnded = true;
                MessageTimer.Reset();
                
                foreach (var bunny in Subject.GetEntities<Monster>().ToList())
                    Subject.RemoveEntity(bunny);
    
                Subject.Morph("26022");
            }
        }
        
        if (CheckTouchTimer.IntervalElapsed)
        {
            var players = Subject.GetEntities<Aisling>().Where(p => p.IsAlive).ToList();
            var bunnies = Subject.GetEntities<Monster>().Where(m => m.IsAlive && BunnyNames.Contains(m.Name)).ToList();

            foreach (var bunny in bunnies)
            {
                foreach (var player in players)
                    if ((bunny.X == player.X) && (bunny.Y == player.Y))
                    {
                        OnBunnyTouchPlayer(bunny, player);
                        return;
                    }
            }
        }

        
        if (!AnnounceStart)
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                if (aisling.Effects.Contains("mount"))
                    aisling.Effects.Dispel("mount");
                
                var point = new Point(aisling.X, aisling.Y);
                if (point != PlayerStartPoint)
                    aisling.WarpTo(PlayerStartPoint);
                
                if (!PlayStartSound)
                {
                    aisling.Client.SendSound(174, false);
                    PlayStartSound = true;
                }
            }
        }
        
        if (MessageTimer.IntervalElapsed && !AnnounceStart)
        {
            if (Subject.Name == "Undine Hopocalypse")
                Subject.Morph("26021");

            GameLevel++;
            SendMessage($"Get the eggs! Level {GameLevel.ToWords()} start!");

            if (!SpawnedBunnies)
            {
                var bunnySpawns = new (string baseId, Point position)[]
                {
                    ("hopscare", new Point(1, 1)),
                    ("whiskerflip", new Point(19, 1)),
                    ("petalpounce", new Point(19, 19)),
                    ("burrowglint", new Point(1, 19))
                };

                foreach ((var baseId, var position) in bunnySpawns)
                {
                    var leveledId = GetLeveledBunnyId(baseId, GameLevel);
                    var bunny = MonsterFactory.Create(leveledId, Subject, position);
                    Subject.AddEntity(bunny, position);
                }

                SpawnEggs();
                SpawnedBunnies = true;
            }

            AnnounceStart = true;
            GameEnded = false;
        }
    }
    
    private string GetLeveledBunnyId(string baseId, int level)
    {
        var clampedLevel = Math.Clamp(level, 1, 5);
        return $"{baseId}{clampedLevel}";
    }

    private void OnBunnyTouchPlayer(Creature bunny, Aisling player)
    {
        const string LEGEND_KEY = "hopmaze";

        if (player.MapInstance.InstanceId != "undine")
        {
            var mapInstance = SimpleCache.Get<MapInstance>("undine");
            player.TraverseMap(mapInstance, new Point(21, 26)); 
        }
        
        RecordStats(player);

        player.SendMessage($"{bunny.Name} caught you and took the stolen eggs back!");
        player.Inventory.RemoveByTemplateKey("undinechickenegg");
        player.Inventory.RemoveByTemplateKey("undinegoldenchickenegg");
        
        player.Client.SendSound(176, false);
        
        var existing = player.Legend
                             .FirstOrDefault(m => m.Key.EqualsI(LEGEND_KEY));

        var previousLevel = 0;
        if (existing != null)

        {
            var m = MyRegex().Match(existing.Text);
            if (m.Success)
                previousLevel = int.Parse(m.Value);
        }

        if (GameLevel > previousLevel)            // upgrade only when higher
        {
            if (player.Legend.ContainsKey("hopmaze"))
                player.Legend.Remove("hopmaze", out _);
            
            player.Legend.AddUnique(new LegendMark(
                $"Reached Level {GameLevel} of Hopocalypse",
                LEGEND_KEY,
                MarkIcon.Victory,
                MarkColor.Pink,
                1,
                GameTime.Now));
        }

        player.Trackers.Enums.TryGetValue(out HopocalypseRewards rewardLevel);

        if ((GameLevel >= 3) && (rewardLevel == HopocalypseRewards.None))
        {
            var chickBackpack = ItemFactory.Create("chickbackpack");
            player.GiveItemOrSendToBank(chickBackpack);
            player.SendMessage("Received Chick Backpack for reaching Hopocalypse Level 3+!");
            rewardLevel = HopocalypseRewards.ChickBackPack;
            player.Trackers.Enums.Set(rewardLevel);
            player.Client.SendSound(168, false);
        }

        if ((GameLevel >= 5) && (rewardLevel != HopocalypseRewards.CadburryBackPack))
        {
            var cadburryBackpack = ItemFactory.Create("cadburrybackpack");
            player.GiveItemOrSendToBank(cadburryBackpack);
            player.SendMessage("Received Cadburry Backpack for reaching Hopocalypse Level 5+!");
            player.Trackers.Enums.Set(HopocalypseRewards.CadburryBackPack);
            player.Client.SendSound(168, false);
        }
    }

    private void RecordStats(Aisling player)
    {
        var leaderboard = LeaderboardStorage.Value;
        leaderboard.UpdateBoard(player.Name, player.Inventory.CountOfByTemplateKey("undinechickenegg"), player.Inventory.CountOfByTemplateKey("undinegoldenchickenegg"), GameLevel);
    }
    
    private void SpawnEggs()
    {
        var bounds = Subject.Template.Bounds;

        for (var x = bounds.Left; x <= bounds.Right; x++)
        {
            for (var y = bounds.Top; y <= bounds.Bottom; y++)
            {
                var point = new Point(x, y);
                
                if (Subject.Template.IsWall(point) || point is (20, 9) || point is (0, 9))
                    continue;
                
                var isCorner = ((x == 1) && (y == 1))
                               || ((x == 19) && (y == 1))
                               || ((x == 19) && (y == 19))
                               || ((x == 1) && (y == 19));

                if (isCorner)
                {
                    var item = ItemFactory.Create("undinegoldenchickenegg");
                    var groundItem = new GroundItem(item, Subject, point);
                    Subject.AddEntity(groundItem, point);
                }
                else
                {
                    var item = ItemFactory.Create("undinechickenegg");
                    var groundItem = new GroundItem(item, Subject, point);
                    Subject.AddEntity(groundItem, point);
                }
                
            }
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\d+")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}