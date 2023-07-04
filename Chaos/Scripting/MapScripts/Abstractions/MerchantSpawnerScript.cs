using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class MerchantSpawnerScript : MapScriptBase
{
    private readonly IMerchantFactory MerchantFactory;

    private readonly ISimpleCache SimpleCache;

    private IIntervalTimer? SpawnTimer;
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract string MerchantTemplateKey { get; set; }
    public abstract int SpawnIntervalMs { get; set; }

    protected MerchantSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
    }

    private Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point));

        return point;
    }

    public override void Update(TimeSpan delta)
    {
        SpawnTimer ??= new IntervalTimer(TimeSpan.FromMilliseconds(SpawnIntervalMs));

        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
        {
            var countOfAllMerchants = Subject.GetEntities<Merchant>()
                                             .Count(obj => obj.Template.TemplateKey.EqualsI(MerchantTemplateKey));

            var maxSpawns = Math.Min(MaxAmount - countOfAllMerchants, MaxPerSpawn);

            // Generate a random number of spawns between 0 and maxSpawns (exclusive)
            var random = new Random();
            var spawnAmount = random.Next(0, maxSpawns);

            for (var i = 0; i < spawnAmount; i++)
            {
                var point = GenerateSpawnPoint(Subject);
                var merchant = MerchantFactory.Create(MerchantTemplateKey, Subject, point);
                Subject.AddObject(merchant, point);
            }
        }
    }
}