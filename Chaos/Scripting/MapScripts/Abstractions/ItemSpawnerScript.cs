using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class ItemSpawnerScript : MapScriptBase
{
    private readonly IItemFactory ItemFactory;

    private readonly ISimpleCache SimpleCache;

    private IIntervalTimer? SpawnTimer;
    public abstract string ItemTemplateKey { get; set; }
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract int SpawnChance { get; set; }
    public abstract int SpawnIntervalMs { get; set; }

    protected ItemSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }

    private Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point) || !selectedMap.IsWalkable(point, CreatureType.Normal));

        return point;
    }

    public override void Update(TimeSpan delta)
    {
        SpawnTimer ??= new IntervalTimer(TimeSpan.FromMilliseconds(SpawnIntervalMs));

        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
        {
            var allCountOfItems = Subject.GetEntities<GroundItem>().Count(obj => obj.Item.Template.TemplateKey.EqualsI(ItemTemplateKey));

            var maxSpawns = Math.Min(MaxAmount - allCountOfItems, MaxPerSpawn);

            if (maxSpawns <= 0)
                return;
            
            maxSpawns++;

            var spawnAmount = Random.Shared.Next(1, maxSpawns);

            if (!IntegerRandomizer.RollChance(SpawnChance))
                return;

            for (var i = 0; i < spawnAmount; i++)
            {
                var point = GenerateSpawnPoint(Subject);
                var item = ItemFactory.Create(ItemTemplateKey);
                var groundItem = new GroundItem(item, Subject, point);

                Subject.AddEntity(groundItem, point);
            }
        }
    }
}