using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Medusa;

public sealed class MedusaMoveToTargetScript : MonsterScriptBase
{
    private DateTime _lastTargetInteractionTime = DateTime.UtcNow;

    /// <inheritdoc />
    public MedusaMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>().Any())
            return;

        var script = Subject.Script.As<MedusaPhaseScript>();

        if (script!.InPhase)
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        if (distance > 1)
        {
            Subject.Pathfind(Target);
        }
        else if (distance == 1)
        {
            var direction = Target.DirectionalRelationTo(Subject);
            Subject.Turn(direction);
        }
        else
        {
            Subject.Wander();
        }

        // Check if Medusa has been unable to reach the target for 10 seconds
        if (DateTime.UtcNow.Subtract(_lastTargetInteractionTime).TotalSeconds >= 10)
        {
            if (distance > 1)
            {
                var options = new AoeShapeOptions
                {
                    Source = new Point(Target.X, Target.Y),
                    Range = 1
                };

                var points = AoeShape.AllAround.ResolvePoints(options).ToList();
                var randomPoint = points.PickRandom();
                if (Map.IsWalkable(randomPoint, collisionType: Subject.Type))
                    Subject.WarpTo(randomPoint);
            }
            else
            {
                // Choose a new target
                Target = Map.GetEntities<Aisling>()
                    .OrderBy(x => x.ManhattanDistanceFrom(Subject))
                    .FirstOrDefault();
            }

            // Reset the timer after warping or selecting a new target
            _lastTargetInteractionTime = DateTime.UtcNow;
        }
        else
        {
            // Reset the timer if Medusa moved
            ResetAttackTimerIfMoved();
            if (distance <= 1)
            {
                _lastTargetInteractionTime = DateTime.UtcNow;
            }
        }
    }

    private void ResetAttackTimerIfMoved()
    {
        var now = DateTime.UtcNow;
        var lastWalk = Subject.Trackers.LastWalk;
        var lastTurn = Subject.Trackers.LastTurn;
        var walkedRecently = lastWalk.HasValue && (now.Subtract(lastWalk.Value).TotalMilliseconds < Subject.Template.MoveIntervalMs);
        var turnedRecently = lastTurn.HasValue && (now.Subtract(lastTurn.Value).TotalMilliseconds < Subject.Template.MoveIntervalMs);

        if (walkedRecently || turnedRecently)
        {
            Subject.WanderTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }
}