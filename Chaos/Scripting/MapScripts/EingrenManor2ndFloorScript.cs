using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class EingrenManor2NdFloorScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer UpdateTimer;
    private readonly Random Random = new();
    private readonly ISimpleCache SimpleCache;
    
    // List of map template keys
    private readonly string[] MapKeys =
    [
        "manor_library",
        "manor_study",
        "manor_study_2",
        "manor_kitchen",
        "manor_kitchen_2",
        "manor_commons",
        "manor_storage",
        "manor_depot",
        "manor_bedroom",
        "manor_bedroom_2",
        "manor_bedroom_3",
        "manor_bunks",
        "manor_master_suite"
    ];
    
    /// <inheritdoc />
    public EingrenManor2NdFloorScript(MapInstance subject, ISimpleCache simpleCache,IMonsterFactory monsterFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    }

    /// <inheritdoc />
    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling)
            return;

        var hasSpawns = Subject.GetEntities<Monster>().Any(x => x.PetOwner is null);
        if (hasSpawns)
            return;

        var monsterTypes = new[] { "enragedbanshee", "annoyedbanshee", "flowingbanshee" };
        
        for (var i = 0; i < 10; i++)
        {
            var randomMonsterType = monsterTypes[Random.Next(0, monsterTypes.Length)];
            var point = Subject.GetRandomWalkablePoint();
            var monster = MonsterFactory.Create(randomMonsterType, Subject, point);
            Subject.AddEntity(monster, point);

            if (IntegerRandomizer.RollChance(10))
            {
                var bossSpawn = MonsterFactory.Create("banshee", Subject, point);
                Subject.AddEntity(bossSpawn, point);
            }
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);
        
        if (UpdateTimer.IntervalElapsed)
        {
            var hasMonsters = Subject.GetEntities<Monster>().Any(x => x.PetOwner is null);
            var hasPlayers = Subject.GetEntities<Aisling>().ToList().Any();

            if (Subject.GetEntities<Aisling>()
                .Any(x => x.Trackers.Counters.CounterGreaterThanOrEqualTo("bansheekills", 100)))
            {
                var rectangle = new Rectangle(25, 3, 2, 2);

                foreach (var member in Subject.GetEntities<Aisling>())
                {
                    var mapInstance = SimpleCache.Get<MapInstance>("manor_main_hall");

                    Point newPoint;
                    do
                    {
                        newPoint = rectangle.GetRandomPoint();
                    } while (!mapInstance.IsWalkable(newPoint, member.Type));

                    member.Trackers.Counters.Remove("bansheekills", out _);
                    member.Trackers.Enums.Set(ManorLouegieStage.CompletedQuest);
                    member.TraverseMap(mapInstance, newPoint);
                }
            }

            if (!hasMonsters && hasPlayers)
            {
                // Randomly select a map key
                var selectedMapKey = MapKeys[Random.Shared.Next(MapKeys.Length)];
                var mapInstance = SimpleCache.Get<MapInstance>(selectedMapKey);
                var point = mapInstance.GetRandomWalkablePoint();
                
                foreach (var player in Subject.GetEntities<Aisling>())
                {
                    player.TraverseMap(mapInstance, point);
                }
            }   
        }
    }
}