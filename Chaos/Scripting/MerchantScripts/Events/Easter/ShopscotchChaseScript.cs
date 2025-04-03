using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events.Easter;

public class ShopscotchChaseScript(Merchant subject) : MerchantScriptBase(subject)
{
    private readonly IIntervalTimer WalkTimer = new IntervalTimer(TimeSpan.FromSeconds(1.5), false);
    private readonly IIntervalTimer ChaseTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
    private readonly IIntervalTimer RestTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly IIntervalTimer ShoutTimer = new IntervalTimer(TimeSpan.FromSeconds(8), false);

    private static readonly Rectangle ChaseBounds = new(18, 25, 7, 8);
    private const string CHICKEN_NPC_TEMPLATE_KEY = "nugget";

    private readonly IPathOptions ChasePathOptions = PathOptions.Default.ForCreatureType(subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true,
        IgnoreWalls = false
    };
    private bool IsResting;

    public override void Update(TimeSpan delta)
    {
        ShoutTimer.Update(delta);
        WalkTimer.Update(delta);

        var chicken = Subject.MapInstance
            .GetEntities<Merchant>()
            .FirstOrDefault(m => m is { IsAlive: true, Template.TemplateKey: CHICKEN_NPC_TEMPLATE_KEY });

        if (chicken == null)
            return;

        if (IsResting)
        {
            RestTimer.Update(delta);

            if (ShoutTimer.IntervalElapsed)
                Subject.Say(RandomRestLine());

            if (RestTimer.IntervalElapsed)
            {
                IsResting = false;
                ChaseTimer.Reset();
            }

            return;
        }

        // Active chase phase
        ChaseTimer.Update(delta);

        if (ShoutTimer.IntervalElapsed)
            Subject.Say(RandomChaseLine());

        if (ChaseTimer.IntervalElapsed)
        {
            IsResting = true;
            RestTimer.Reset();
            return;
        }

        if (WalkTimer.IntervalElapsed)
        {
            var fleePoint = GetRandomPointAwayFrom(ChaseBounds, new Point(chicken.X, chicken.Y));
            Subject.Pathfind(fleePoint, 0, ChasePathOptions);
        }
    }

    private string RandomChaseLine()
    {
        var lines = new[]
        {
            "EEEEK!!",
            "SHE'S BEHIND ME!!",
            "HELP!! SHE'S GONNA PECK ME!!",
            "HOP FASTER, HOP FASTER!!",
            "NOT THE BEAK!!",
            "I DIDN'T MEAN TO TAKE THE EGG!!",
            "I'M TOO FLUFFY TO DIE!!",
            "GO AWAY, NUGGET!!",
            "SHE'S SERIOUS THIS TIME!!",
            "I CAN'T OUTRUN FEATHERS!!"
        };

        return lines.PickRandom();
    }

    private string RandomRestLine()
    {
        var lines = new[]
        {
            "*gasp gasp*",
            "*pant*",
            "I need..a carrot break...",
            "Why meee...",
            "I think I lost a shoe",
            "*deep breaths*",
            "That chick is unhinged.",
            "Sooo fluffy.. sooo tired...",
            "Next time I wear sneakers."
        };

        return lines.PickRandom();
    }

    private static Point GetRandomPointAwayFrom(Rectangle bounds, Point from, int minDistance = 3)
    {
        var possiblePoints = new List<Point>();

        for (var x = bounds.Left; x <= bounds.Right; x++)
        {
            for (var y = bounds.Top; y <= bounds.Bottom; y++)
            {
                var point = new Point(x, y);
                if (point.ManhattanDistanceFrom(from) >= minDistance)
                {
                    possiblePoints.Add(point);
                }
            }
        }

        return possiblePoints.Count > 0
            ? possiblePoints[Random.Shared.Next(possiblePoints.Count)]
            : new Point(bounds.Left, bounds.Top); // fallback
    }
}
