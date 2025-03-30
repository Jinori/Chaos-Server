using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Direction = Chaos.Geometry.Abstractions.Definitions.Direction;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildCombatScript : DialogScriptBase
{
    private static readonly Random Random = new();
    private readonly IMonsterFactory MonsterFactory;

    private readonly Dictionary<string, Func<Aisling, List<Point>>> SkillMappings;

    public GuildCombatScript(Dialog subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;

        SkillMappings = new Dictionary<string, Func<Aisling, List<Point>>>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "skillcombat_cleave", src => GetAoePoints(AoeShape.Cleave, src, 1)
            },
            {
                "skillcombat_frontalcone_range3", src => GetAoePoints(AoeShape.FrontalCone, src, 3)
            },
            {
                "skillcombat_frontalcone_range4", src => GetAoePoints(AoeShape.FrontalCone, src, 4)
            },
            {
                "skillcombat_frontalcone_frontandcascaderange2", src => GetAoePoints(
                    AoeShape.FrontalCone,
                    src,
                    2,
                    1)
            },
            {
                "skillcombat_frontalcone_frontandcascaderange3", src => GetAoePoints(
                    AoeShape.FrontalCone,
                    src,
                    3,
                    1)
            },
            {
                "skillcombat_frontaldiamond_frontandcascaderange4", src => GetAoePoints(
                    AoeShape.FrontalDiamond,
                    src,
                    4,
                    1)
            },
            {
                "skillcombat_frontaldiamond_range3", src => GetAoePoints(AoeShape.FrontalDiamond, src, 3)
            },
            {
                "skillcombat_threeinrowfront", src => GetAoePoints(AoeShape.FrontalCone, src, 1)
            },
            {
                "skillcombat_fourinfront", src => GetAoePoints(AoeShape.Front, src, 4)
            },
            {
                "skillcombat_threeinfront", src => GetAoePoints(AoeShape.Front, src, 3)
            },
            {
                "skillcombat_twoinfront", src => GetAoePoints(AoeShape.Front, src, 2)
            },
            {
                "skillcombat_oneinfront", src => GetAoePoints(AoeShape.Front, src, 1)
            },
            {
                "skillcombat_allaround", src => GetAoePoints(AoeShape.AllAround, src, 1)
            },
            {
                "skillcombat_allaround2", src => GetAoePoints(AoeShape.AllAround, src, 2)
            },
            {
                "skillcombat_allaround3", src => GetAoePoints(AoeShape.AllAround, src, 3)
            },
            {
                "skillcombat_allaround4", src => GetAoePoints(AoeShape.AllAround, src, 4)
            },
            {
                "skillcombat_allaround5", src => GetAoePoints(AoeShape.AllAround, src, 5)
            },
            {
                "skillcombat_randomspots", src => GetRandomPointsCentered(new Point(src.X, src.Y))
            }
        };
    }

    private static void ClearMonsters(Aisling source)
    {
        foreach (var monster in source.MapInstance
                                      .GetEntities<Monster>()
                                      .ToList())
            source.MapInstance.RemoveEntity(monster);
    }

    private static List<Point> GetAoePoints(
        AoeShape shape,
        Aisling source,
        int range,
        int offsetX = 0,
        int offsetY = 0)
        => shape.ResolvePoints(
                    new AoeShapeOptions
                    {
                        Source = new Point(source.X + offsetX, source.Y + offsetY),
                        Range = range,
                        Direction = source.Direction
                    })
                .ToList();

    private static List<Point> GetRandomPointsCentered(
        Point center,
        int width = 6,
        int height = 6,
        int count = 5)
    {
        var halfWidth = width / 2;
        var halfHeight = height / 2;
        var topLeft = new Point(center.X - halfWidth, center.Y - halfHeight);

        var points = new HashSet<Point>();

        while (points.Count < count)
        {
            var x = Random.Next(topLeft.X, topLeft.X + width);
            var y = Random.Next(topLeft.Y, topLeft.Y + height);

            var point = new Point(x, y);

            if (point != center)
                points.Add(point);
        }

        return [..points];
    }

    public override void OnDisplaying(Aisling source)
    {
        if (!SkillMappings.TryGetValue(Subject.Template.TemplateKey, out var getPoints))
            return;

        ClearMonsters(source);
        WarpToPosition(source);

        var points = getPoints(source);
        SpawnMonsters(source, points);

        Subject.Close(source);
    }

    private void SpawnMonsters(Aisling source, List<Point> points)
    {
        foreach (var point in points)
        {
            if (point == new Point(source.X, source.Y))
                continue;

            var mob = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
            mob.Direction = Direction.Right;
            source.MapInstance.AddEntity(mob, point);
        }
    }

    private void WarpToPosition(Aisling source)
    {
        source.WarpTo(new Point(63, 29));
        source.Turn(Direction.Right);
    }
}