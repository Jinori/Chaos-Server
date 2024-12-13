using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

public class SpawnErbieMapScript : MapScriptBase
{
    private const int SPAWN_INTERVAL_MINUTES = 1;
    public const int UPDATE_INTERVAL_MS = 1000; // Increased for realistic update intervals

    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer? SpawnTimer;
    private readonly IIntervalTimer? UpdateTimer;

    public SpawnErbieMapScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS));

        SpawnTimer = new RandomizedIntervalTimer(
            TimeSpan.FromMinutes(SPAWN_INTERVAL_MINUTES),
            50,
            RandomizationType.Positive,
            false);
    }

    private void SpawnErbies()
    {
        // Count existing "Lost Erbie" monsters on the map
        var existingErbies = Subject.GetEntities<Monster>()
                                    .Count(x => x.Name == "Lost Erbie");

        // Calculate how many more need to be spawned to reach 8
        var amountToSpawn = 6 - existingErbies;

        // Only proceed if more Erbies need to be spawned
        if (amountToSpawn > 0)
            for (var i = 0; i < amountToSpawn; i++)
                if (Subject.TryGetRandomWalkablePoint(out var point))
                {
                    // Create and add a new "Lost Erbie" monster to the map
                    var monster = MonsterFactory.Create("mtmerry_losterbie", Subject, point);
                    Subject.AddEntity(monster, monster);
                }
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);
        SpawnTimer?.Update(delta);

        if (UpdateTimer!.IntervalElapsed)
            if (SpawnTimer!.IntervalElapsed)
                SpawnErbies();
    }
}