using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

public class ChristmasFrostyBombScript : MapScriptBase
{
    private readonly IIntervalTimer BombSpawnTimer;
    private readonly IIntervalTimer ReindeerSpawnTimer;
    private readonly IRectangle BombSpawnArea;
    private readonly List<Point> ReindeerSpawnPoints;
    private readonly IMonsterFactory MonsterFactory;
    private readonly Random RandomGenerator;
    private readonly HashSet<Point> UsedReindeerSpawnPoints; // Tracks used spawn points

    public ChristmasFrostyBombScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        RandomGenerator = new Random();

        // Define spawn areas
        BombSpawnArea = new Rectangle(left: 8, top: 8, width: 17, height: 13);
        ReindeerSpawnPoints = GenerateReindeerSpawnPoints();
        UsedReindeerSpawnPoints = new HashSet<Point>();

        // Timers for spawning bombs and reindeer
        BombSpawnTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(6), 40, RandomizationType.Positive, false);
        ReindeerSpawnTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(10), 60, RandomizationType.Positive, false);
    }

    public override void Update(TimeSpan delta)
    {
        // Check if there are any Aislings on the map
        if (!Subject.GetEntities<Aisling>().Any())
            return;

        // Update timers
        BombSpawnTimer.Update(delta);
        ReindeerSpawnTimer.Update(delta);

        // Spawn bombs if timer elapsed
        if (BombSpawnTimer.IntervalElapsed)
            SpawnPresentBombs();

        // Spawn crazed reindeer if timer elapsed
        if (ReindeerSpawnTimer.IntervalElapsed)
            SpawnCrazedReindeer();
    }

    private void SpawnPresentBombs()
    {
        var playerCount = Subject.GetEntities<Aisling>().Count();
        var bombCount = GetBombCount(playerCount);

        for (var i = 0; i < bombCount; i++)
        {
            Point spawnPoint;

            do
                spawnPoint = BombSpawnArea.GetRandomPoint();
            while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling));

            // Create a monster bomb
            var bomb = MonsterFactory.Create("smiley_blob_bomb", Subject, spawnPoint);

            Subject.AddEntity(bomb, bomb);
        }
    }

    private void SpawnCrazedReindeer()
    {
        // Random number of reindeer to spawn
        var reindeerCount = RandomGenerator.Next(1, 9); // Between 1 and 8 inclusive

        var availableSpawnPoints = new List<Point>(ReindeerSpawnPoints.Except(UsedReindeerSpawnPoints));

        for (var i = 0; i < reindeerCount; i++)
        {
            if (availableSpawnPoints.Count == 0)
                break; // No more available points to spawn

            Point spawnPoint;

            // Select a random point from available spawn points
            do
            {
                spawnPoint = availableSpawnPoints[RandomGenerator.Next(availableSpawnPoints.Count)];
            } while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling));

            // Mark the point as used
            UsedReindeerSpawnPoints.Add(spawnPoint);

            // Create a crazed reindeer
            var reindeer = MonsterFactory.Create("crazed_reindeer", Subject, spawnPoint);

            Subject.AddEntity(reindeer, reindeer);

            // Remove the point from the available list
            availableSpawnPoints.Remove(spawnPoint);
        }

        // Clear used spawn points when all reindeer have exited
        ClearUsedSpawnPointsIfNeeded();
    }

    private List<Point> GenerateReindeerSpawnPoints()
    {
        // Generate points in a vertical line from (7, 8) to (7, 20)
        var points = new List<Point>();
        for (var y = 8; y <= 20; y++)
        {
            points.Add(new Point(7, y));
        }
        return points;
    }

    private void ClearUsedSpawnPointsIfNeeded()
    {
        // Clear used points if all reindeer at the exit line (25, 8 to 25, 20) are removed
        if (!Subject.GetEntities<Monster>()
                   .Where(monster => monster.Name == "crazed_reindeer")
                   .Any(monster => ReindeerSpawnPoints.Contains(new Point(monster.X, monster.Y))))
        {
            UsedReindeerSpawnPoints.Clear();
        }
    }

    private int GetBombCount(int playerCount)
    {
        // Scale bombs inversely with player count
        return Math.Max(1, 40 - playerCount);
    }
}
