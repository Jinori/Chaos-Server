using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.Easter;

public class WhiskerflipScript(Monster subject) : BunnyMazeBaseScript(subject)
{
    private const int CHASE_DISTANCE_THRESHOLD = 3;
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

        var playerPos = new Point(Target.X, Target.Y);
        var myPos = new Point(Subject.X, Subject.Y);
        var distance = myPos.EuclideanDistanceFrom(playerPos);

        if ((distance >= CHASE_DISTANCE_THRESHOLD) && Subject.MoveTimer.IntervalElapsed)
            Subject.Pathfind(Target, 0, Options);
        else
            if (Subject.WanderTimer.IntervalElapsed) 
                Subject.Wander(Options);
    }
}