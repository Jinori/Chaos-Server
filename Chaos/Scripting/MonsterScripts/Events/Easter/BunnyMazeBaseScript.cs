using Chaos.Extensions;
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
    private enum BunnyState { Chase, Scatter, Frightened }

    private static readonly TimeSpan ChaseDuration = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan ScatterDuration = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan FrightenedDuration = TimeSpan.FromSeconds(12);
    private static readonly TimeSpan HistoryInterval = TimeSpan.FromSeconds(1);
    private const int MAX_POSITION_HISTORY = 5;
    private const int FRIGHT_SOUND_ID = 178;
    private const int FRIGHTENED_SPRITE_ID = 262;

    private BunnyState CurrentState = BunnyState.Chase;
    private bool PlayFrightSound;
    private readonly IIntervalTimer ChaseTimer = new IntervalTimer(ChaseDuration, false);
    private readonly IIntervalTimer ScatterTimer = new IntervalTimer(ScatterDuration, false);
    private readonly IIntervalTimer FrightenedTimer = new IntervalTimer(FrightenedDuration, false);
    private readonly IIntervalTimer HistoryTimer = new IntervalTimer(HistoryInterval, false);
    private readonly Queue<Point> RecentPositions = new();

    protected readonly IPoint HomePoint = new Point(10, 8);

    protected IPathOptions ChaseOptions => PathOptions.Default.ForCreatureType(Subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true,
        IgnoreWalls = false
    };

    protected IPathOptions FrightOptions => PathOptions.Default.ForCreatureType(Subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true,
        IgnoreWalls = true,
    };

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        EnsureTarget();

        if ((Target == null) || !Map.GetEntities<Aisling>().Any()) 
            return;

        HistoryTimer.Update(delta);
        if (HistoryTimer.IntervalElapsed)
            RecordTargetPosition();

        if (AnyBunnyFrightened() && CurrentState != BunnyState.Frightened)
            EnterFrightenedState();

        switch (CurrentState)
        {
            case BunnyState.Chase:
                ChaseTimer.Update(delta);
                DoChase();
                if (ChaseTimer.IntervalElapsed) SwitchState();
                break;

            case BunnyState.Scatter:
                ScatterTimer.Update(delta);
                DoScatter();
                if (ScatterTimer.IntervalElapsed) SwitchState();
                break;

            case BunnyState.Frightened:
                FrightenedTimer.Update(delta);
                DoFrightened();
                if (FrightenedTimer.IntervalElapsed) SwitchState();
                break;
        }
    }

    private void RecordTargetPosition()
    {
        if (Target == null) return;

        if (RecentPositions.Count >= MAX_POSITION_HISTORY)
            RecentPositions.Dequeue();

        RecentPositions.Enqueue(new Point(Target.X, Target.Y));
    }

    protected void EnsureTarget()
    {
        if (Target is { IsAlive: true } && Target.OnSameMapAs(Subject)) 
            return;

        Target = Map.GetEntities<Aisling>().FirstOrDefault(a => a.IsAlive && a.OnSameMapAs(Subject));
    }

    private bool AnyBunnyFrightened() =>
        Map.GetEntities<Monster>().Any(x => x.Trackers.Counters.ContainsKey("Frightened"));

    private void EnterFrightenedState()
    {
        foreach (var bunny in Map.GetEntities<Monster>())
        {
            bunny.Sprite = FRIGHTENED_SPRITE_ID;
            bunny.Display();
        }

        FrightenedTimer.Reset();
        CurrentState = BunnyState.Frightened;
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
                ExitFrightenedState();
                break;
        }
    }

    private void ExitFrightenedState()
    {
        foreach (var bunny in Map.GetEntities<Monster>().Where(x => x.Trackers.Counters.ContainsKey("Frightened")))
        {
            bunny.Trackers.Counters.Remove("Frightened", out _);
            bunny.Sprite = bunny.Template.Sprite;
            bunny.Display();
        }

        CurrentState = BunnyState.Chase;
        ChaseTimer.Reset();
        ScatterTimer.Reset();
        PlayFrightSound = false;
    }

    protected virtual void DoScatter()
    {
        if (Subject.EuclideanDistanceFrom(HomePoint) > 2 && Subject.MoveTimer.IntervalElapsed)
            Subject.Pathfind(HomePoint, 0, ChaseOptions, true);
    }

    protected virtual void DoFrightened()
    {
        if (Subject.MoveTimer.IntervalElapsed && Subject.ManhattanDistanceFrom(HomePoint) >= 1)
            Subject.Pathfind(HomePoint, 0, FrightOptions, true);

        if (!PlayFrightSound && Target is Aisling ais)
        {
            ais.Client.SendSound(FRIGHT_SOUND_ID, false);
            PlayFrightSound = true;
        }
    }

    protected abstract void DoChase();

    public Point PredictAhead(Creature target, int tilesAhead)
    {
        if (RecentPositions.Count < 2)
            return PredictByDirection(target.Direction, target, tilesAhead);

        (var dx, var dy) = GetVelocityVectorFromHistory();
        return PredictFromOffset(target, dx, dy, tilesAhead);
    }

    public Point PredictAheadWithTurns(Creature target, int tilesAhead)
    {
        if (RecentPositions.Count < 2)
            return PredictByDirection(target.Direction, target, tilesAhead);

        (var dx, var dy) = GetVelocityVectorFromHistory();
        var predicted = PredictFromOffset(target, dx, dy, tilesAhead);

        if (((dx != 0) || (dy != 0)) && TryGetTurnPrediction(target, tilesAhead, dx, out var turnPrediction))
            return turnPrediction;
        
        return IsWalkable(predicted) ? predicted : new Point(target.X, target.Y);
    }

    private (int dx, int dy) GetVelocityVectorFromHistory()
    {
        var positions = RecentPositions.ToArray();
        var from = positions[^2];
        var to = positions[^1];
        return (Math.Sign(to.X - from.X), Math.Sign(to.Y - from.Y));
    }

    private Point PredictFromOffset(Creature target, int dx, int dy, int tilesAhead)
    {
        var predicted = new Point(target.X + dx * tilesAhead, target.Y + dy * tilesAhead);
        return IsWalkable(predicted) ? predicted : new Point(target.X, target.Y);
    }

    private Point PredictByDirection(Direction direction, Creature target, int tilesAhead)
    {
        (var dx, var dy) = direction switch
        {
            Direction.Up => (0, -1),
            Direction.Down => (0, 1),
            Direction.Left => (-1, 0),
            Direction.Right => (1, 0),
            _ => (0, 0)
        };
        return PredictFromOffset(target, dx, dy, tilesAhead);
    }

    private bool TryGetTurnPrediction(Creature target, int tilesAhead, int dx, out Point prediction)
    {
        prediction = new Point(target.X, target.Y);

        // Determine perpendicular directions based on horizontal or vertical movement
        var directions = dx != 0
            ? new[] { (0, -1), (0, 1) } // Horizontal movement, check vertical turns
            : new[] { (-1, 0), (1, 0) }; // Vertical movement, check horizontal turns

        foreach ((var offsetX, var offsetY) in directions)
        {
            var sideTile = new Point(target.X + offsetX, target.Y + offsetY);
            if (!IsWalkable(sideTile)) continue;

            var turn = new Point(target.X + offsetX * tilesAhead, target.Y + offsetY * tilesAhead);
            if (IsWalkable(turn))
            {
                prediction = turn;
                return true;
            }
        }

        return false;
    }


    private bool IsWalkable(Point point) => Map.IsWalkable(point, true, false, collisionType: Subject.Type);
}
