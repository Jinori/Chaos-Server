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
    private readonly IIntervalTimer BombSpawnTimer; // Timer for bombs
    private readonly IIntervalTimer ReindeerSpawnTimer; // Timer for reindeer
    private readonly IIntervalTimer MazeSpawnTimer; // Timer for maze
    private readonly IIntervalTimer PrizeBoxSpawnTimer; // Timer for prize boxes
    private readonly IRectangle BombSpawnArea;
    private readonly List<Point> ReindeerSpawnPoints;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IItemFactory ItemFactory;
    private readonly Random RandomGenerator;
    private readonly HashSet<Point> UsedReindeerSpawnPoints; // Tracks used spawn points

    public ChristmasFrostyBombScript(MapInstance subject, IMonsterFactory monsterFactory, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        MonsterFactory = monsterFactory;
        RandomGenerator = new Random();

        // Define spawn areas
        BombSpawnArea = new Rectangle(left: 8, top: 8, width: 17, height: 13);
        ReindeerSpawnPoints = GenerateReindeerSpawnPoints();
        UsedReindeerSpawnPoints = new HashSet<Point>();

        // Timers for bombs, reindeer, maze, and prize boxes
        BombSpawnTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(300), 85, RandomizationType.Negative, false);
        ReindeerSpawnTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(10), 60, RandomizationType.Positive, false);
        MazeSpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(30)); // Maze every 30 seconds
        PrizeBoxSpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(45)); // Prize boxes every 45 seconds
    }

    public override void Update(TimeSpan delta)
    {
        // Check if there are any Aislings on the map
        if (!Subject.GetEntities<Aisling>().Any())
            return;

        // Update bomb spawn timer
        BombSpawnTimer.Update(delta);
        if (BombSpawnTimer.IntervalElapsed)
        {
            SpawnSingleBomb();
        }

        // Update reindeer timer
        ReindeerSpawnTimer.Update(delta);
        if (ReindeerSpawnTimer.IntervalElapsed)
        {
            SpawnCrazedReindeer();
        }

        // Update maze spawn timer
        MazeSpawnTimer.Update(delta);
        if (MazeSpawnTimer.IntervalElapsed)
        {
            SpawnMaze();
        }

        // Update prize box spawn timer
        PrizeBoxSpawnTimer.Update(delta);
        if (PrizeBoxSpawnTimer.IntervalElapsed)
        {
            SpawnPrizeBoxes();
        }
    }

    private void SpawnSingleBomb()
    {
        Point spawnPoint;

        do
        {
            spawnPoint = BombSpawnArea.GetRandomPoint();
        } 
        while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling) || IsBombAtPosition(spawnPoint));

        // Create a monster bomb
        var bomb = MonsterFactory.Create("smiley_blob_bomb", Subject, spawnPoint);

        Subject.AddEntity(bomb, bomb);
    }

    private bool IsBombAtPosition(Point position)
    {
        // Check if there is already a bomb at the position
        return Subject.GetEntities<Monster>()
                      .Any(monster => monster.Template.TemplateKey == "smiley_blob_bomb" && new Point(monster.X, monster.Y) == position);
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

    private void SpawnMaze()
    {
        // Clear existing maze creatures before creating a new one
        RemoveExistingMaze();

        // Number of maze creatures
        var mazeCreatureCount = RandomGenerator.Next(10, 25);

        for (var i = 0; i < mazeCreatureCount; i++)
        {
            Point spawnPoint;

            do
            {
                spawnPoint = BombSpawnArea.GetRandomPoint();
            } 
            while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling) || IsCreatureAtPosition(spawnPoint));

            // Create a maze creature
            var mazeCreature = MonsterFactory.Create("carnun_wall", Subject, spawnPoint);

            Subject.AddEntity(mazeCreature, mazeCreature);
        }
    }

    private void SpawnPrizeBoxes()
    {
        
        // Random number of prize boxes to spawn
        var prizeBoxCount = RandomGenerator.Next(2, 3);

        for (var i = 0; i < prizeBoxCount; i++)
        {
            Point spawnPoint;

            do
            {
                spawnPoint = BombSpawnArea.GetRandomPoint();
            } 
            while (!Subject.IsWalkable(spawnPoint, CreatureType.Aisling) || IsCreatureAtPosition(spawnPoint));

            // Spawn the prize box item
            var prizeBox = ItemFactory.Create("mountmerrybox");
            var groundItem = new GroundItem(prizeBox, Subject, spawnPoint);
            Subject.AddEntity(groundItem, spawnPoint);
        }
    }

    private bool IsCreatureAtPosition(Point position)
    {
        // Check if there is already a creature at the position
        return Subject.GetEntities<Monster>()
                      .Any(monster => new Point(monster.X, monster.Y) == position);
    }

    private void RemoveExistingMaze()
    {
        // Remove all existing maze creatures
        var mazeCreatures = Subject.GetEntities<Monster>()
                                   .Where(monster => monster.Template.TemplateKey == "carnun_wall")
                                   .ToList();

        foreach (var creature in mazeCreatures)
        {
            Subject.RemoveEntity(creature);
        }
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
                   .Where(monster => monster.Template.TemplateKey == "crazed_reindeer")
                   .Any(monster => ReindeerSpawnPoints.Contains(new Point(monster.X, monster.Y))))
        {
            UsedReindeerSpawnPoints.Clear();
        }
    }
}
