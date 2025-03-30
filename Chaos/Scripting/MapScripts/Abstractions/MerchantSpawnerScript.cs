using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class MerchantSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory) : MapScriptBase(subject)
{
    private IIntervalTimer? SpawnTimer;
    public abstract int MaxAmount { get; set; }
    public abstract int MaxPerSpawn { get; set; }
    public abstract string MerchantTemplateKey { get; set; }

    public abstract int MinDistanceFromWall { get; set; }
    public abstract int SpawnIntervalMs { get; set; }

    private Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (IsTooCloseToObstacle(selectedMap, point));

        return point;
    }

    private bool IsTooCloseToObstacle(MapInstance selectedMap, Point point)
    {
        if (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point) || !selectedMap.IsWalkable(point, CreatureType.Normal))
            return true;

        Point[] directions =
        [
            new(point.X - 1, point.Y),
            new(point.X + 1, point.Y),
            new(point.X, point.Y - 1),
            new(point.X, point.Y + 1)
        ];

        foreach (var direction in directions)
            if (selectedMap.IsWall(direction)
                || selectedMap.IsBlockingReactor(direction)
                || !selectedMap.IsWalkable(direction, CreatureType.Normal))
                return true;

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

            if (maxSpawns == 0)
                return;

            maxSpawns++;

            if (!IntegerRandomizer.RollChance(10))
                return;

            var spawnAmount = Random.Shared.Next(1, maxSpawns);

            for (var i = 0; i < spawnAmount; i++)
            {
                var point = GenerateSpawnPoint(Subject);
                var merchant = merchantFactory.Create(MerchantTemplateKey, Subject, point);
                Subject.AddEntity(merchant, point);
            }
        }
    }
}