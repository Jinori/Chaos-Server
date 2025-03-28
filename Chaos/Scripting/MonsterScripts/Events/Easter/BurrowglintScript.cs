using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.Easter;

public class BurrowglintScript(Monster subject) : BunnyMazeBaseScript(subject)
{
    private const int PREDICTION_DISTANCE = 2;

    private IPathOptions Options => PathOptions.Default with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true,
        IgnoreWalls = false
    };
    
    protected override void DoChase()
    {
        if (Target == null)
            return;

        var direction = GetTargetDirection(Target);
        var projectedPoint = PredictAhead(Target, direction, PREDICTION_DISTANCE);

        // Find Hopscare (must be alive and on same map)
        var hopscare = Map
                       .GetEntities<Monster>()
                       .FirstOrDefault(m => m.Template.TemplateKey.Equals("hopscare", StringComparison.OrdinalIgnoreCase));

        if (hopscare == null)
        {
            if (Subject.MoveTimer.IntervalElapsed) 
                Subject.Pathfind(projectedPoint, 0, Options);
            
            return;
        }

        // Use vector from Hopscare to projected point
        var vectorX = projectedPoint.X - hopscare.X;
        var vectorY = projectedPoint.Y - hopscare.Y;

        var targetFlank = new Point(hopscare.X + 2 * vectorX, hopscare.Y + 2 * vectorY);

        if (!Map.IsWalkable(targetFlank, Subject.Type))
        {
            // Fallback to player position if target is blocked
            targetFlank = new Point(Target.X, Target.Y);
        }

        if (Subject.MoveTimer.IntervalElapsed) 
            Subject.Pathfind(targetFlank, 0, Options);
    }
}