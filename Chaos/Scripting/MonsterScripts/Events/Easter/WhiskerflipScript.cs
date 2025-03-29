using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.Easter;

public class WhiskerflipScript(Monster subject) : BunnyMazeBaseScript(subject)
{
    private const int CHASE_DISTANCE_THRESHOLD = 2;
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
        
        var distance = Subject.ManhattanDistanceFrom(Target);

        if ((distance >= CHASE_DISTANCE_THRESHOLD))
        {
            if (Subject.MoveTimer.IntervalElapsed) 
                Subject.Pathfind(Target, 0, Options);   
        }
        else
            if (Subject.WanderTimer.IntervalElapsed) 
                Subject.Wander(Options);
    }
}