using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class NobisMazeTrapRoomScript : MapScriptBase
{
    public NobisMazeTrapRoomScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        if (Subject.Template.TemplateKey == "20506")
        {
            var rectangle = new Rectangle(
                1,
                10,
                27,
                11);

            var monster = monsterFactory.Create("nm_trap", Subject, new Point());
            var points = new HashSet<Point>();
            var count = rectangle.Width * rectangle.Height / 6;

            for (var i = 0; i < count; i++)
            {
                var point = GenerateSpawnPoint(Subject, rectangle);
                points.Add(point);
            }

            foreach (var point in points)
            {
                var trap = reactorTileFactory.Create(
                    "nm_roomtrap",
                    Subject,
                    point,
                    owner: monster);

                Subject.SimpleAdd(trap);
            }
        }

        if (Subject.Template.TemplateKey == "6585")
        {
            var rectangle = new Rectangle(
                1,
                10,
                23,
                19);

            var monster = monsterFactory.Create("nm_trap", Subject, new Point());
            var points = new HashSet<Point>();
            var count = rectangle.Width * rectangle.Height / 7;

            for (var i = 0; i < count; i++)
            {
                var point = GenerateSpawnPoint(Subject, rectangle);
                points.Add(point);
            }

            foreach (var point in points)
            {
                var trap = reactorTileFactory.Create(
                    "nm_roomtrap",
                    Subject,
                    point,
                    owner: monster);

                Subject.SimpleAdd(trap);
            }
        }
    }

    private Point GenerateSpawnPoint(MapInstance selectedMap, Rectangle rectangle)
    {
        Point point;

        do
            point = rectangle.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point));

        return point;
    }
}