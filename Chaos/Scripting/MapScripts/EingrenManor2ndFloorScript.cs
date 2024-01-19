using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class EingrenManor2ndFloorScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer UpdateTimer;
    private readonly Random Random = new();
    private readonly ISimpleCache _simpleCache;
    
    // List of map template keys
    private readonly string[] MapKeys = {
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
    };
    
    /// <inheritdoc />
    public EingrenManor2ndFloorScript(MapInstance subject, ISimpleCache simpleCache,IMonsterFactory monsterFactory)
        : base(subject)
    {
        _simpleCache = simpleCache;
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

        var monsterTypes = new[] { "airphasedghost", "earthphasedghost", "firephasedghost", "waterphasedghost" };
        
        for (var i = 0; i < 10; i++)
        {
            var randomMonsterType = monsterTypes[Random.Next(0, monsterTypes.Length)];
            var point = Subject.GetRandomWalkablePoint();
            var monster = MonsterFactory.Create(randomMonsterType, Subject, point);
            Subject.AddEntity(monster, point);
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

            if (!hasMonsters && hasPlayers)
            {
                // Randomly select a map key
                var selectedMapKey = MapKeys[Random.Shared.Next(MapKeys.Length)];
                var mapInstance = _simpleCache.Get<MapInstance>(selectedMapKey);
                var point = mapInstance.GetRandomWalkablePoint();
                
                foreach (var player in Subject.GetEntities<Aisling>())
                {
                    player.TraverseMap(mapInstance, point);
                }
            }   
        }
    }
}