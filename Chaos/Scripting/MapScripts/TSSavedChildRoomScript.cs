using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories;
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
        var points = new HashSet<Point>();
        var randomNumber = new Random().Next(1, 4);
        var count = randomNumber;

        for (var i = 0; i < count; i++)
        {
            var point = GenerateSpawnPoint(Subject);
            points.Add(point);
        }

        foreach (var point in points)
        {
            var tschild = ReactorTileFactory.Create(
                "ts_child",
                Subject,
                point,
                owner: merchant);

            Subject.SimpleAdd(tschild);
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