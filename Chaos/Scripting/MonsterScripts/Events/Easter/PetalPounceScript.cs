using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.Easter;

public class PetalpounceScript(Monster subject) : BunnyMazeBaseScript(subject)
{
    private const int PREDICTION_DISTANCE = 4;
    private IPathOptions Options => PathOptions.Default.ForCreatureType(Subject.Type) with
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
        var predicted = PredictAhead(Target, direction, PREDICTION_DISTANCE);

        if (!Map.IsWalkable(predicted, true, false, true))
            predicted = new Point(Target.X, Target.Y);

        if (Subject.MoveTimer.IntervalElapsed)
            Subject.Pathfind(predicted, 0, Options, true);
    }
}