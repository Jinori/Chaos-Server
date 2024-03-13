using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.karlopos;

public class KarloposTrapRoomScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IReactorTileFactory ReactorTileFactory;

    public KarloposTrapRoomScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        MonsterFactory = monsterFactory;

        var monster = MonsterFactory.Create("karlopos_path_monster", Subject, new Point());
        var points = new HashSet<Point>();
        var count = Subject.Template.Width * Subject.Template.Height / 20;

        for (var i = 0; i < count; i++)
        {
            var point = GenerateSpawnPoint(Subject);
            points.Add(point);
        }

        foreach (var point in points)
        {
            var trap = ReactorTileFactory.Create(
                "pf_roomtrap",
                Subject,
                point,
                owner: monster);

            Subject.SimpleAdd(trap);
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