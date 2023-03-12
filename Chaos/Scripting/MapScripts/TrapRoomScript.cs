using Chaos.Containers;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class TrapRoomScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IReactorTileFactory ReactorTileFactory;
    
    public TrapRoomScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        MonsterFactory = monsterFactory;

        var monster = MonsterFactory.Create("pf_path_monster", Subject, new Point());
        var points = new HashSet<Point>();
        var count = Subject.Template.Width * Subject.Template.Height / 3;

        for (var i = 0; i < count; i++)
        {
            var point = subject.Template.Bounds.RandomPoint();
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
}