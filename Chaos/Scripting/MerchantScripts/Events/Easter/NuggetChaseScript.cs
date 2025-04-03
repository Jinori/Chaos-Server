using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events.Easter;

public class NuggetChaseScript(Merchant subject) : MerchantScriptBase(subject)
{
    private readonly IIntervalTimer WalkTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly IIntervalTimer PeckTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false); 

    private readonly IPathOptions ChaseOptions = PathOptions.Default.ForCreatureType(subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true,
        IgnoreWalls = false
    };
    private static readonly Rectangle ChaseBounds = new(18, 25, 7, 8);

    private const string SHOPSCOTCH_TEMPLATE_KEY = "shopscotch";

    public override void Update(TimeSpan delta)
    {
        WalkTimer.Update(delta);
        PeckTimer.Update(delta);

        var target = Subject.MapInstance
            .GetEntities<Merchant>()
            .FirstOrDefault(m => m is { IsAlive: true, Template.TemplateKey: SHOPSCOTCH_TEMPLATE_KEY });

        if (target == null)
            return;

        var nuggetPoint = new Point(Subject.X, Subject.Y);
        var targetPoint = new Point(target.X, target.Y);

        if (nuggetPoint.ManhattanDistanceFrom(targetPoint) == 1)
        {
            FaceToward(Subject, target);

            if (PeckTimer.IntervalElapsed)
            {
                Subject.Chant(RandomPeckLine());
                PeckTimer.Reset();
            }

            return;
        }

        if (WalkTimer.IntervalElapsed)
        {
            var clampedTarget = ClampToRectangle(targetPoint, ChaseBounds);
            Subject.Pathfind(clampedTarget, 0, ChaseOptions);
        }
    }

    private void FaceToward(Merchant from, Merchant to)
    {
        if (to.X > from.X)
            from.Turn(Direction.Right);
        else if (to.X < from.X)
            from.Turn(Direction.Left);
        else if (to.Y > from.Y)
            from.Turn(Direction.Down);
        else if (to.Y < from.Y)
            from.Turn(Direction.Up);
    }

    private string RandomPeckLine()
    {
        var lines = new[]
        {
            "PECK!!",
            "BAWK BAWK!!",
            "*angry flapping*",
            "*peck peck peck*",
        };

        return lines[Random.Shared.Next(lines.Length)];
    }

    private static Point ClampToRectangle(Point point, Rectangle bounds)
    {
        var x = Math.Clamp(point.X, bounds.Left, bounds.Right);
        var y = Math.Clamp(point.Y, bounds.Top, bounds.Bottom);
        return new Point(x, y);
    }
}
