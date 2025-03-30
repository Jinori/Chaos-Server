using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Pathfinding;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MonsterScripts.Events;

public class SnagglesMonster(Monster subject, IMerchantFactory merchantFactory, IMonsterFactory monsterFactory, ISimpleCache simpleCache) : MonsterScriptBase(subject)
{
    private bool Tagged;
    private int TagCount;
    private bool sweet;
    private readonly IIntervalTimer TagDelayTimer = new IntervalTimer(TimeSpan.FromSeconds(3));
    private readonly IIntervalTimer RandomWalkInterval = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(350), 130);
    private readonly IIntervalTimer SpawnAddsTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
    private readonly IIntervalTimer SpawnsKillTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    private readonly IIntervalTimer WalkToSweetsTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);

    public override void OnPublicMessage(Creature source, string message)
    {
        base.OnPublicMessage(source, message);
        
        if (source is not Aisling aisling)
            return;

        if (message is not "I give up!") 
            return;
        
        var instance = simpleCache.Get<MapInstance>("loures_1_floor_restaurant");
        aisling.TraverseMap(instance, new Point(5, 5));
    }
    
    public override void Update(TimeSpan delta)
    {
        // Update walk interval
        RandomWalkInterval.Update(delta);
        TagDelayTimer.Update(delta);
        SpawnAddsTimer.Update(delta);
        SpawnsKillTimer.Update(delta);
        WalkToSweetsTimer.Update(delta);

        
        var sweets = (Subject.MapInstance.GetEntities<GroundItem>().Where(x => x.Name is "Sweet Buns" or "Sugar").ToList());
        if (sweets.Count > 0)
        {
            sweet = true;
        }
        else
        {
            sweet = false;
        }
        
        if (WalkToSweetsTimer.IntervalElapsed)
        {
            if (sweets.Count > 0)
            {
                if (Subject.EuclideanDistanceFrom(sweets[0]) > 1)
                    Subject.Pathfind(sweets[0], 1);
                else
                {
                    Subject.Say("Yum!");
                    Subject.MapInstance.RemoveEntity(sweets[0]);
                }
            }
        }

        if (sweet)
            return;
        
        if (SpawnsKillTimer.IntervalElapsed)
        {
            var spawns = Subject.MapInstance.GetEntities<Monster>().Where(x => x.Template.TemplateKey == "snagglecottoncandy" && (DateTime.UtcNow - x.Creation) >= TimeSpan.FromSeconds(15)).ToList();
            foreach (var spawn in spawns)
            {
                Subject.MapInstance.RemoveEntity(spawn);
            }
        }

        if (SpawnAddsTimer.IntervalElapsed)
            SpawnAdds();
        
        if (RandomWalkInterval.IntervalElapsed && !Tagged && TagCount < 3)
        {
            if (!IsAislingNearby()) // Check if Aisling is next to Subject
            {
                Wander();
            }
            else
            {
                TagCount++;
                Tagged = true;
                TagDelayTimer.Reset();
                Subject.Say("Stay away from my sweets!");
                foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>())
                {
                    aisling.SendOrangeBarMessage($"You tagged Snaggles {TagCount.ToWords()} times! Tag him three times to win!");
                }
            }
        }

        if (TagDelayTimer.IntervalElapsed && Tagged && TagCount < 3)
        {
            Subject.MapInstance.TryGetRandomWalkablePoint(out var point);
            if (point != null) 
                Subject.WarpTo(point);
            Subject.Say("Won't catch me again! Take this!");
            SpawnAdds();
            Tagged = false;
        }
        
        if (TagCount >= 3)
        {
            Subject.MapInstance.Morph("2701");
            var merchant = merchantFactory.Create("snagglesthesweetsnatcher", Subject.MapInstance, new Point(11, 12));
            Subject.MapInstance.AddEntity(merchant, new Point(11, 12));
            merchant.Say("I thought I was the fastest..");
            var spawns = Subject.MapInstance.GetEntities<Monster>().Where(x => x.Template.TemplateKey == "snagglecottoncandy").ToList();
            foreach (var spawn in spawns)
            {
                Subject.MapInstance.RemoveEntity(spawn);
            }
            Subject.MapInstance.RemoveEntity(Subject);
        }
    }

    private void SpawnAdds()
    {
        Subject.Say("I'm calling for some sweet backup!");
        var spawnPoints = Subject.GenerateIntercardinalPoints(radius: 2)
            .Skip(4)
            .Where(point => Map.IsWithinMap(point));

        var spawns = spawnPoints.Select(spawnPoint => monsterFactory.Create("snaggleCottonCandy", Subject.MapInstance, spawnPoint))
            .ToList();

        Subject.MapInstance.AddEntities(spawns);
    }
    
    public virtual void Wander()
    {
        var options = new PathOptions
        {
            IgnoreWalls = false
        };

        // Get nearby Aislings (players)
        var nearbyAislings = Subject.MapInstance
            .GetEntitiesWithinRange<Aisling>(Subject, 6)
            .ThatCanSee(Subject)
            .ToList();

        // Convert Aisling entities to positions
        var aislingPositions = nearbyAislings.Select(a => new Point(a.X, a.Y)).ToList();

        // Get non-wall positions that are at least 4 tiles away from walls
        var safeFromWalls = Subject.SpiralSearch(2)
            .Where(x => !Subject.MapInstance.IsWall(x))
            .ToHashSet();

        // Get the map bounds and ensure the NPC stays at least 3 tiles away
        var mapBounds = Subject.MapInstance.Template.Bounds;

        bool IsWithinSafeMapBounds(Point point) =>
            point.X >= mapBounds.Left + 3 && point.X <= mapBounds.Right - 3 &&
            point.Y >= mapBounds.Top + 3 && point.Y <= mapBounds.Bottom - 3;

        // Merge blocked points with Aisling positions
        var nearbyUnwalkablePoints = nearbyAislings.Concat(options.BlockedPoints).ToHashSet();
        options.BlockedPoints = nearbyUnwalkablePoints;

        var possibleDirections = new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

        // Ensure directions are walkable, not near walls, and within map bounds
        var walkableDirections = possibleDirections
            .Where(dir =>
            {
                var nextPoint = Subject.DirectionalOffset(dir);
                return Subject.MapInstance.IsWalkable(nextPoint, collisionType: Subject.Type) &&
                       safeFromWalls.Contains(nextPoint) &&
                       IsWithinSafeMapBounds(nextPoint);
            })
            .ToList();

        // Filter out directions that move toward an Aisling
        var safeDirections = walkableDirections
            .Where(dir =>
                !MovesTowardAisling(new Point(Subject.X, Subject.Y), Subject.DirectionalOffset(dir), aislingPositions))
            .ToList();

        // If no safe directions exist, fallback to any valid direction
        var finalDirections = safeDirections.Any() ? safeDirections : walkableDirections;

        if (!finalDirections.Any())
            return; // No available moves

        var chosenDirection = finalDirections[Random.Shared.Next(finalDirections.Count)];

        Subject.Walk(chosenDirection);
    }

    private bool MovesTowardAisling(Point currentPosition, Point nextPosition, IEnumerable<Point> aislingPositions)
    {
        return aislingPositions.Any(aislingPos =>
            nextPosition.EuclideanDistanceFrom(aislingPos) < currentPosition.EuclideanDistanceFrom(aislingPos));
    }
    
    private bool IsAislingNearby()
    {
        var nearbyAislings = Subject.MapInstance
            .GetEntitiesWithinRange<Aisling>(Subject, 1)
            .ToList();

        return nearbyAislings.Any(aisling =>
            IsAdjacent(new Point(Subject.X, Subject.Y), new Point(aisling.X, aisling.Y)));
    }

    private bool IsAdjacent(Point subjectPosition, Point aislingPosition)
    {
        return (Math.Abs(subjectPosition.X - aislingPosition.X) == 1 && subjectPosition.Y == aislingPosition.Y) ||
               (Math.Abs(subjectPosition.Y - aislingPosition.Y) == 1 && subjectPosition.X == aislingPosition.X);
    }


}