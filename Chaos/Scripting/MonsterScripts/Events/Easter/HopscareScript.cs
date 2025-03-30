using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.Easter;

public class HopscareScript(Monster subject) : BunnyMazeBaseScript(subject)
{
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
        
        if (Subject.MoveTimer.IntervalElapsed) 
            Subject.Pathfind(Target, 0, Options, true);
    }
}