using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.Easter;

public abstract class BunnyMazeBaseScript(Monster subject) : MonsterScriptBase(subject)
{
    public enum BunnyState
    {
        Chase,
        Scatter,
        Frightened,
    }
    
    private IPathOptions Options => PathOptions.Default with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true,
        IgnoreWalls = false
    };
    
    private BunnyState CurrentState = BunnyState.Chase;
    
    private readonly IIntervalTimer ChaseTimer = new IntervalTimer(TimeSpan.FromSeconds(20), false);
    private readonly IIntervalTimer ScatterTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly IIntervalTimer FrightenedTimer = new IntervalTimer(TimeSpan.FromSeconds(8), false);

    protected readonly IPoint HomePoint = new Point(10, 8);

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        EnsureTarget();

        if ((Target == null) || !Map.GetEntities<Aisling>().Any())
            return;

        if (Map.GetEntities<Monster>().Any(x => x.Trackers.Counters.ContainsKey("Frightened")) && (CurrentState != BunnyState.Frightened))
        {
            foreach (var bunny in Map.GetEntities<Monster>())
            {
                bunny.Sprite = 262;   
                bunny.Display();
            }

            FrightenedTimer.Reset();
            CurrentState = BunnyState.Frightened;   
        }

        if (CurrentState == BunnyState.Chase)
        {
            ChaseTimer.Update(delta);
            
            DoChase();
            
            if (ChaseTimer.IntervalElapsed)
                SwitchState();
        }
        else if (CurrentState == BunnyState.Scatter)
        {
            ScatterTimer.Update(delta);
            
            DoScatter();
            
            if (ScatterTimer.IntervalElapsed)
                SwitchState();
        }
        else if (CurrentState == BunnyState.Frightened)
        {
            FrightenedTimer.Update(delta);

            DoFrightened();
            
            if (FrightenedTimer.IntervalElapsed)
                SwitchState();
        }
    }

    protected void EnsureTarget()
    {
        if (Target is { IsAlive: true } && Target.OnSameMapAs(Subject))
            return;

        Target = Map.GetEntities<Aisling>()
                    .FirstOrDefault(a => a.IsAlive && a.OnSameMapAs(Subject));
    }
    
    private void SwitchState()
    {
        switch (CurrentState)
        {
            case BunnyState.Chase:
                CurrentState = BunnyState.Scatter;
                ScatterTimer.Reset();

                break;
            case BunnyState.Scatter:
                CurrentState = BunnyState.Chase;
                ChaseTimer.Reset();

                break;
            
            case BunnyState.Frightened:
                foreach (var bunny in Map.GetEntities<Monster>().Where(x => x.Trackers.Counters.ContainsKey("Frightened")))
                {
                    bunny.Trackers.Counters.Remove("Frightened", out _);
                    bunny.Sprite = bunny.Template.Sprite;
                    bunny.Display();
                }
                
                CurrentState = BunnyState.Chase;
                ChaseTimer.Reset();
                ScatterTimer.Reset();
                
                break;
        }
    }

    protected virtual void DoScatter()
    {
        if ((Subject.EuclideanDistanceFrom(HomePoint) > 2) && Subject.MoveTimer.IntervalElapsed)
            Subject.Pathfind(HomePoint, 0, Options);
        
        else if (Subject.WanderTimer.IntervalElapsed)
            Subject.Wander(Options);
    }

    protected virtual void DoFrightened()
    {
        if (Subject.MoveTimer.IntervalElapsed && (Subject.ManhattanDistanceFrom(HomePoint) >= 1))
            Subject.Pathfind(HomePoint, 0, Options);
    }

    protected abstract void DoChase();

    protected Direction GetTargetDirection(Creature target) => target.Direction;

    public Point PredictAhead(Creature target, Direction direction, int tilesAhead)
    {
        var dx = 0;
        var dy = 0;

        switch (direction)
        {
            case Direction.Up: dy = -1; break;
            case Direction.Down: dy = 1; break;
            case Direction.Left: dx = -1; break;
            case Direction.Right: dx = 1; break;
        }

        return new Point(target.X + dx * tilesAhead, target.Y + dy * tilesAhead);
    }
}
