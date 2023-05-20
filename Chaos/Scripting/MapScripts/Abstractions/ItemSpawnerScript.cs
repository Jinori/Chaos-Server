using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class ItemSpawnerScript : MapScriptBase
{
    private readonly IItemFactory ItemFactory;
    public abstract string ItemTemplateKey { get; set; }
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract int SpawnIntervalMs { get; set; }
    
    private IIntervalTimer? SpawnTimer;

    protected ItemSpawnerScript(MapInstance subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void Update(TimeSpan delta)
    {
        SpawnTimer ??=  new IntervalTimer(TimeSpan.FromMilliseconds(SpawnIntervalMs));
        
        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
        {
            var currentCount = Subject.GetEntities<GroundItem>().Count(obj => obj.Item.Template.TemplateKey.EqualsI(ItemTemplateKey));
            var spawnAmount = Math.Min(MaxAmount - currentCount, MaxPerSpawn);
            
            for (var i = 0; i < spawnAmount; i++)
            {
                var point = GenerateSpawnPoint();
                var item = ItemFactory.Create(ItemTemplateKey);
                var groundItem = new GroundItem(item, Subject, point);
                
                Subject.AddObject(groundItem, point);
            }
        }
    }

    private Point GenerateSpawnPoint()
    {
        Point point;

        do
            point = Subject.Template.Bounds.GetRandomPoint();
        while (Subject.IsWall(point) || Subject.IsBlockingReactor(point));

        return point;
    }
}