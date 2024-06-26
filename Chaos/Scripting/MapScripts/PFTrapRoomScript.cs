using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class TrapRoomScript : MapScriptBase
{
    public TrapRoomScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        var monster = monsterFactory.Create("pf_path_monster", Subject, new Point());
        var points = new HashSet<Point>();
        var count = Subject.Template.Width * Subject.Template.Height / 6;

        for (var i = 0; i < count; i++)
        {
            var point = GenerateSpawnPoint(Subject);
            points.Add(point);
        }

        foreach (var point in points)
        {
            var trap = reactorTileFactory.Create(
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