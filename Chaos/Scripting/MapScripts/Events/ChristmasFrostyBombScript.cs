using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MapScripts.Events;

public class ChristmasFrostyBombScript : MapScriptBase
{
    private readonly IRectangle BombSpawnArea;

    private readonly IIntervalTimer BombSpawnTimer;

    private readonly IIntervalTimer DifficultyTimer;
    private readonly IItemFactory ItemFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer PrizeBoxSpawnTimer;
    private readonly Random RandomGenerator;
    private readonly List<Point> ReindeerSpawnPoints;
    private readonly IIntervalTimer ReindeerSpawnTimer;
    private readonly IIntervalTimer RewardTimer;
    private readonly TimeSpan TimerDuration = TimeSpan.FromMinutes(3);
    private readonly HashSet<Point> UsedReindeerSpawnPoints;
    private int BombCount = 10; // Start with 10 bomb

    private ScriptStage CurrentStage;
    private int ReindeerCount = 2; // Start with 2 reindeer
    private DateTime TimerStart;
    private bool Warn10Sec;
    private bool Warn1Min;
    private bool Warn2Min;
    private bool Warn30Sec;

    public ChristmasFrostyBombScript(MapInstance subject, IMonsterFactory monsterFactory, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        MonsterFactory = monsterFactory;
        RandomGenerator = new Random();

        BombSpawnArea = new Rectangle(
            7,
            7,
            19,
            15);
        ReindeerSpawnPoints = GenerateReindeerSpawnPoints();
        UsedReindeerSpawnPoints = new HashSet<Point>();

        BombSpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(2));
        ReindeerSpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(6));
        PrizeBoxSpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(45));
        DifficultyTimer = new IntervalTimer(TimeSpan.FromSeconds(15)); // Adjust difficulty every 15 seconds
        RewardTimer = new IntervalTimer(TimeSpan.FromSeconds(1)); // Adjust difficulty every 15 seconds
        TimerStart = DateTime.UtcNow;

        CurrentStage = ScriptStage.Dormant;
    }

    private void ClearUsedSpawnPointsIfNeeded()
    {
        // Check if there are any reindeer on the defined spawn points
        var hasReindeerOnSpawnPoints = Subject.GetEntities<Monster>()
                                              .Where(monster => monster.Template.TemplateKey == "crazed_reindeer")
                                              .Any(monster => ReindeerSpawnPoints.Contains(new Point(monster.X, monster.Y)));

        // If no reindeer are found, clear the used spawn points
        if (!hasReindeerOnSpawnPoints)
            UsedReindeerSpawnPoints.Clear();
    }

    private List<Point> GenerateReindeerSpawnPoints()
    {
        var points = new List<Point>();

        for (var y = 8; y <= 20; y++)
            points.Add(new Point(7, y));

        return points;
    }

    private bool IsBombAtPosition(Point position)
        => Subject.GetEntities<Monster>()
                  .Any(monster => (monster.Template.TemplateKey == "smiley_blob_bomb") && (new Point(monster.X, monster.Y) == position));

    private bool IsCreatureAtPosition(Point position)
        => Subject.GetEntities<Monster>()
                  .Any(monster => new Point(monster.X, monster.Y) == position);

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        if (ScriptStage.Dormant == CurrentStage)
        {
            var currentTime = DateTime.UtcNow;
            var elapsedTime = currentTime - TimerStart;
            var remainingTime = TimerDuration - elapsedTime;
            aisling.SendOrangeBarMessage($"Next round will begin in {remainingTime.Humanize()}");
        }
    }

    private void ResetEventState()
    {
        Warn2Min = false;
        Warn1Min = false;
        Warn30Sec = false;
        Warn10Sec = false;

        BombCount = 10; // Reset to default bomb count
        ReindeerCount = 2; // Reset to default reindeer count

        var presents = Subject.GetEntities<GroundItem>()
                              .Where(x => (x.Name == "Mount Merry Box") && BombSpawnArea.Contains(new Point(x.X, x.Y)))
                              .ToList();

        foreach (var present in presents)
            Subject.RemoveEntity(present);

        // Reset the state of the event
        UsedReindeerSpawnPoints.Clear();
    }

    private void SpawnCrazedReindeer()
    {
        var availableSpawnPoints = new List<Point>(ReindeerSpawnPoints.Except(UsedReindeerSpawnPoints));

        if (availableSpawnPoints.Count == 0)
            return; // Exit if still no available points (safety check)

        Point spawnPoint;

        do
            spawnPoint = availableSpawnPoints[RandomGenerator.Next(availableSpawnPoints.Count)];
        while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling));

        UsedReindeerSpawnPoints.Add(spawnPoint);

        var reindeer = MonsterFactory.Create("crazed_reindeer", Subject, spawnPoint);
        Subject.AddEntity(reindeer, reindeer);
    }

    private void SpawnPrizeBoxes()
    {
        var playerCount = Subject.GetEntities<Aisling>()
                                 .Count(a => BombSpawnArea.Contains(new Point(a.X, a.Y)));

        var prizeBoxCount = playerCount / 3;

        if (prizeBoxCount < 1)
            prizeBoxCount = 1;

        if (prizeBoxCount > 4)
            prizeBoxCount = 4;

        for (var i = 0; i < prizeBoxCount; i++)
        {
            Point spawnPoint;

            do
                spawnPoint = BombSpawnArea.GetRandomPoint();
            while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling) || IsCreatureAtPosition(spawnPoint));

            var prizeBox = ItemFactory.Create("mountmerrybox");
            var groundItem = new GroundItem(prizeBox, Subject, spawnPoint);
            Subject.AddEntity(groundItem, spawnPoint);
        }
    }

    private void SpawnSingleBomb()
    {
        Point spawnPoint;

        do
            spawnPoint = BombSpawnArea.GetRandomPoint();
        while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling) || IsBombAtPosition(spawnPoint));

        var bomb = MonsterFactory.Create("smiley_blob_bomb", Subject, spawnPoint);
        Subject.AddEntity(bomb, bomb);
    }

    public override void Update(TimeSpan delta)
    {
        var currentTime = DateTime.UtcNow;
        var elapsedTime = currentTime - TimerStart;
        var remainingTime = TimerDuration - elapsedTime;
        BombSpawnTimer.Update(delta);
        DifficultyTimer.Update(delta);
        PrizeBoxSpawnTimer.Update(delta);
        ReindeerSpawnTimer.Update(delta);

        var aislings = Subject.GetEntities<Aisling>()
                              .ToList();

        var npc = Subject.GetEntities<Merchant>()
                         .FirstOrDefault(); // Replace with specific NPC if needed

        if (npc == null)
            return; // Exit if no NPC is found

        switch (CurrentStage)
        {
            case ScriptStage.Dormant:
                // Check and issue warnings
                if ((remainingTime <= TimeSpan.FromMinutes(2)) && !Warn2Min)
                {
                    npc.Say("Round will be starting in 2 minutes!");
                    Warn2Min = true;
                } else if ((remainingTime <= TimeSpan.FromMinutes(1)) && !Warn1Min)
                {
                    npc.Say("Round will be starting in 1 minute!");
                    Warn1Min = true;
                } else if ((remainingTime <= TimeSpan.FromSeconds(30)) && !Warn30Sec)
                {
                    npc.Say("Round will be starting in 30 seconds!");
                    Warn30Sec = true;
                } else if ((remainingTime <= TimeSpan.FromSeconds(10)) && !Warn10Sec)
                {
                    npc.Say("Round will be starting in 10 seconds!");
                    Warn10Sec = true;
                }

                if (elapsedTime >= TimerDuration)
                    CurrentStage = ScriptStage.Starting;

                break;

            case ScriptStage.Starting:

                var playersParticipating = Subject.GetEntities<Aisling>()
                                                  .Any(a => BombSpawnArea.Contains(new Point(a.X, a.Y)));

                if (!playersParticipating)
                {
                    CurrentStage = ScriptStage.NoParticipants;

                    return;
                }

                foreach (var aisling in aislings)
                    aisling.SendOrangeBarMessage("Round is starting! Dodge for your life!");

                npc.Say("Round is starting, good luck!");
                CurrentStage = ScriptStage.InProgress;

                break;

            case ScriptStage.InProgress:
                // Check if players are still in the area
                var playersInRectangle = Subject.GetEntities<Aisling>()
                                                .Where(a => BombSpawnArea.Contains(new Point(a.X, a.Y)))
                                                .ToList();

                if (playersInRectangle.IsNullOrEmpty())
                {
                    CurrentStage = ScriptStage.Complete;
                    TimerStart = DateTime.UtcNow; // Reset the timer
                } else
                {
                    // Update difficulty timer
                    DifficultyTimer.Update(delta);
                    RewardTimer.Update(delta);

                    if (RewardTimer.IntervalElapsed)
                        foreach (var player in playersInRectangle)
                            player.Trackers.Counters.AddOrIncrement("frostychallenge");

                    if (DifficultyTimer.IntervalElapsed)
                    {
                        BombCount++; // Increase bomb count

                        if (ReindeerCount < 8)
                            ReindeerCount++; // Increase reindeer count
                    }

                    if (BombSpawnTimer.IntervalElapsed)
                    {
                        if (playersInRectangle.Count > 3)
                        {
                            var adjustedBombCount = Math.Max(12, BombCount - playersInRectangle.Count);

                            for (var i = 0; i < adjustedBombCount; i++)
                                SpawnSingleBomb();
                        } else
                            for (var i = 0; i < BombCount; i++)
                                SpawnSingleBomb();
                    }

                    if (ReindeerSpawnTimer.IntervalElapsed)
                    {
                        ClearUsedSpawnPointsIfNeeded();

                        if (playersInRectangle.Count > 3)
                        {
                            var adjustedReindeerCount = Math.Max(2, ReindeerCount - playersInRectangle.Count / 3);

                            for (var i = 0; i < adjustedReindeerCount; i++)
                                SpawnCrazedReindeer();
                        } else
                            for (var i = 0; i < ReindeerCount; i++)
                                SpawnCrazedReindeer();
                    }

                    if (PrizeBoxSpawnTimer.IntervalElapsed)
                        SpawnPrizeBoxes();
                }

                break;

            case ScriptStage.Complete:
                CurrentStage = ScriptStage.Dormant;

                foreach (var aisling in aislings)
                    aisling.SendOrangeBarMessage("Round is over! No survivors!");
                npc.Say("The round is over! Next round starts in 3 minutes.");
                TimerStart = DateTime.UtcNow; // Restart timer for next round
                ResetEventState();

                break;

            case ScriptStage.NoParticipants:
                CurrentStage = ScriptStage.Dormant;
                TimerStart = DateTime.UtcNow; // Restart timer for next round
                ResetEventState();
                npc.Say("There were no active participants! Next round in 3 minutes.");

                break;
        }
    }

    private enum ScriptStage
    {
        Dormant,
        Starting,
        InProgress,
        Complete,
        NoParticipants
    }
}