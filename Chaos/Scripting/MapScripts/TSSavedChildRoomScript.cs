using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class TSSavedChildRoomScript : MapScriptBase
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly IReactorTileFactory ReactorTileFactory;

    public TSSavedChildRoomScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        MerchantFactory = merchantFactory;

        var merchant = MerchantFactory.Create("tschild", Subject, new Point());

        // Always generate exactly 2 random points
        var points = new HashSet<Point>();

        for (var i = 0; i < 2; i++)
        {
            var point = GenerateSpawnPoint(Subject);
            points.Add(point);
        }

        // Place the reactor on both points
        foreach (var point in points)
        {
            var tsChild = ReactorTileFactory.Create(
                "ts_child",
                Subject,
                point,
                owner: merchant);

            Subject.SimpleAdd(tsChild);
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