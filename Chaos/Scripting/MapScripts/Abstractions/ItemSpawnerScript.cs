using Chaos.Collections;
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
    public abstract List<string> MapsToSpawnItemsOn { get; set; }
    public abstract string ItemTemplateKey { get; set; }
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract int SpawnIntervalMs { get; set; }
    
    private readonly ISimpleCache SimpleCache;
    
    private IIntervalTimer? SpawnTimer;

    protected ItemSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }


    public override void Update(TimeSpan delta)
    {
        SpawnTimer ??= new IntervalTimer(TimeSpan.FromMilliseconds(SpawnIntervalMs));

        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
        {
            var allCountOfItems = MapsToSpawnItemsOn.Sum(
                s => SimpleCache.Get<MapInstance>(s)
                                .GetEntities<GroundItem>()
                                .Count(obj => obj.Item.Template.TemplateKey.EqualsI(ItemTemplateKey)));
            
            var spawnAmount = Math.Min(MaxAmount - allCountOfItems, MaxPerSpawn);

            for (var i = 0; i < spawnAmount; i++)
            {
                var selectedMapId = MapsToSpawnItemsOn.PickRandom();

                var selectedMap = SimpleCache.Get<MapInstance>(selectedMapId);
                var point = GenerateSpawnPoint(selectedMap);
                var item = ItemFactory.Create(ItemTemplateKey);
                var groundItem = new GroundItem(item, Subject, point);

                selectedMap.AddObject(groundItem, point);
            }
        }
    }

    private Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point));

        return point;
    }
}