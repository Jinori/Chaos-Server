using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class MerchantSpawnerScript : MapScriptBase
{
    private readonly IMerchantFactory MerchantFactory;

    private IIntervalTimer? SpawnTimer;
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract string MerchantTemplateKey { get; set; }

    public abstract int MinDistanceFromWall { get; set; }
    public abstract int SpawnIntervalMs { get; set; }

    protected MerchantSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory)
        : base(subject) =>
        MerchantFactory = merchantFactory;

    private IPoint GenerateSpawnPoint(MapTemplate selectedMap) => selectedMap.Bounds.GetRandomPoint(pt => !IsNearWall(pt, selectedMap));

    public bool IsNearWall(IPoint point, MapTemplate selectedMap)
    {
        var length = MinDistanceFromWall * 2 + 1;
        var rect = new Rectangle(point, length, length);

        return rect.GetPoints().Any(pt => selectedMap.IsWall(pt));
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

            if (maxSpawns == 0)
                return;

            maxSpawns++;

            if (!IntegerRandomizer.RollChance(15))
                return;

            var spawnAmount = Random.Shared.Next(1, maxSpawns);

            for (var i = 0; i < spawnAmount; i++)
            {
                var point = GenerateSpawnPoint(Subject.Template);
                var merchant = MerchantFactory.Create(MerchantTemplateKey, Subject, point);
                Subject.AddEntity(merchant, point);
            }
        }
    }
}