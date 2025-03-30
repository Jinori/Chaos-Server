using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class ItemSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache) : MapScriptBase(subject)
{
    private IIntervalTimer? SpawnTimer;
    public abstract string ItemTemplateKey { get; set; }
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract int SpawnChance { get; set; }
    public abstract int SpawnIntervalMs { get; set; }

    private readonly List<string> StPatricksCharmTemplateKeys =
        ["horseshoecharm", "bluemooncharm", "heartcharm", "clovercharm", "potofgoldcharm", "rainbowcharm", "starcharm", "redballooncharm"];

    private Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point) || !selectedMap.IsWalkable(point, collisionType: CreatureType.Normal));

        return point;
    }

    private bool CheckToSpawnEventItems()
    {
       var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.InstanceId);

        return isEventActive switch
        {
            true when StPatricksCharmTemplateKeys.Contains(ItemTemplateKey)  => true,
            false when StPatricksCharmTemplateKeys.Contains(ItemTemplateKey) => false,
            _                                                                => true
        };
    }
    
    public override void Update(TimeSpan delta)
    {
        if (!CheckToSpawnEventItems())
            return;
        
        
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
                var item = itemFactory.Create(ItemTemplateKey);
                var groundItem = new GroundItem(item, Subject, point);

                Subject.AddEntity(groundItem, point);
            }
        }
    }
}