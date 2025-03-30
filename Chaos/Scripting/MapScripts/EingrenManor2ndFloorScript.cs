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

public class EingrenManor2NdFloorScript(MapInstance subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
    : MapScriptBase(subject)
{
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

    private readonly Random Random = new();
    private readonly IIntervalTimer UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    private DateTime? TransitionTime;

    private void CheckForTransition()
    {
        var hasMonsters = Subject.GetEntities<Monster>()
                                 .Any(x => x.PetOwner is null);

        var hasPlayers = Subject.GetEntities<Aisling>()
                                .ToList()
                                .Count
                         != 0;

        if (Subject.GetEntities<Aisling>()
                   .Any(x => x.Trackers.Counters.CounterGreaterThanOrEqualTo("bansheekills", 100)))
        {
            var rectangle = new Rectangle(
                25,
                3,
                2,
                2);

            foreach (var member in Subject.GetEntities<Aisling>())
            {
                var mapInstance = simpleCache.Get<MapInstance>("manor_main_hall");

                Point newPoint;

                do
                    newPoint = rectangle.GetRandomPoint();
                while (!mapInstance.IsWalkable(newPoint, member.Type));

                member.Trackers.Counters.Remove("bansheekills", out _);
                member.Trackers.Enums.Set(ManorLouegieStage.CompletedQuest);
                member.TraverseMap(mapInstance, newPoint);
            }

            return;
        }

        if (!hasMonsters && hasPlayers && (TransitionTime == null))
            TransitionTime = DateTime.UtcNow.AddSeconds(8);
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling)
            return;

        var hasSpawns = Subject.GetEntities<Monster>()
                               .Any(x => x.PetOwner is null);

        if (hasSpawns)
            return;

        SpawnMonsters();
    }

    private void PerformDelayedTransition()
    {
        var selectedMapKey = MapKeys[Random.Shared.Next(MapKeys.Length)];
        var mapInstance = simpleCache.Get<MapInstance>(selectedMapKey);

        if (!mapInstance.TryGetRandomWalkablePoint(out var point))
        {
            var manorHall = simpleCache.Get<MapInstance>("manor_main_hall");

            foreach (var player in Subject.GetEntities<Aisling>())
                player.TraverseMap(manorHall, new Point(20, 3));
        }

        foreach (var player in Subject.GetEntities<Aisling>())
            if (point != null)
                player.TraverseMap(mapInstance, point);
    }

    private void SpawnMonsters()
    {
        var monsterTypes = new[]
        {
            "enragedbanshee",
            "annoyedbanshee",
            "flowingbanshee"
        };

        for (var i = 0; i < 10; i++)
        {
            var randomMonsterType = monsterTypes[Random.Next(0, monsterTypes.Length)];

            if (!Subject.TryGetRandomWalkablePoint(out var point))
                continue;

            var monster = monsterFactory.Create(randomMonsterType, Subject, point);
            Subject.AddEntity(monster, point);

            if (IntegerRandomizer.RollChance(5))
            {
                var bossSpawn = monsterFactory.Create("banshee", Subject, point);
                Subject.AddEntity(bossSpawn, point);
            }
        }
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);

        if (UpdateTimer.IntervalElapsed)
            CheckForTransition();

        if (TransitionTime.HasValue && (DateTime.UtcNow >= TransitionTime.Value))
        {
            PerformDelayedTransition();
            TransitionTime = null;
        }
    }
}