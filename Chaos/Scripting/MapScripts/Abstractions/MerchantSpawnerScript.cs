using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
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

    private IPoint GenerateSpawnPoint(MapTemplate selectedMap)
    {
        var validPoints = GenerateValidSpawnPoints(selectedMap, 3);

        if (validPoints.Count == 0)
            throw new Exception("No valid spawn points!");

        // Randomly pick a valid point
        var index = new Random().Next(validPoints.Count);

        return validPoints[index];
    }

    private List<IPoint> GenerateValidSpawnPoints(MapTemplate selectedMap, int minDistanceFromWall)
    {
        var validPoints = new List<IPoint>();

        for (var x = 0; x < selectedMap.Width; x++)
        {
            for (var y = 0; y < selectedMap.Height; y++)
            {
                IPoint point = new Point(x, y);

                // Exclude walls
                if (selectedMap.IsWall(point))
                    continue;

                // Exclude points near walls
                if (!IsNearWall(point, selectedMap, minDistanceFromWall))
                    validPoints.Add(point);
            }
        }

        return validPoints;
    }

    public bool IsNearWall(IPoint point, MapTemplate selectedMap, int radius)
    {
        for (var x = -radius; x <= radius; x++)
        {
            for (var y = -radius; y <= radius; y++)
            {
                var checkPoint = new Point(point.X + x, point.Y + y);

                if (!selectedMap.IsWithinMap(checkPoint))
                    continue;

                if (selectedMap.IsWall(checkPoint))
                    return true;
            }
        }

        return false;
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
                var point = GenerateSpawnPoint(Subject.Template);
                var merchant = MerchantFactory.Create(MerchantTemplateKey, Subject, point);
                Subject.AddObject(merchant, point);
            }
        }
    }
}