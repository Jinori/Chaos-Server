using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.wilderness;

public class SpawnSilkWormScript : MapScriptBase
{
    public SpawnSilkWormScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        var merchant = merchantFactory.Create("wormspawner", Subject, new Point());
        var points = new HashSet<Point>();
        const int COUNT = 100;

        for (var i = 0; i < COUNT; i++)
        {
            var point = GenerateSpawnPoint(Subject);
            points.Add(point);
        }

        foreach (var wormspawn in points.Select(point => reactorTileFactory.Create(
                     "spawnsilkworm",
                     Subject,
                     point,
                     owner: merchant)))
        {
            Subject.SimpleAdd(wormspawn);
        }
    }

    private static Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point));

        return point;
    }
}