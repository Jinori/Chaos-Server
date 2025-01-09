using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

public class PhoenixMovementScript : MonsterScriptBase
{
    private readonly IIntervalTimer SkyChangeDirectionTimer
        = new RandomizedIntervalTimer(TimeSpan.FromSeconds(1), 100, RandomizationType.Positive);
    
    /// <inheritdoc />
    public PhoenixMovementScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //face random directions in the sky
        //but dont move
        if (Map.LoadedFromInstanceId.ContainsI("sky"))
        {
            SkyChangeDirectionTimer.Update(delta);

            if (SkyChangeDirectionTimer.IntervalElapsed)
            {
                var randomDirection = (Direction)Random.Shared.Next(4);

                if (Subject.Direction != randomDirection)
                    Subject.Turn(randomDirection);
            }

            return;
        }

        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        switch (distance)
        {
            case > 5:
            {
                var inverseRelationalDirection = Subject.DirectionalRelationTo(Target);

                var pointsAroundTarget = Target.GenerateCardinalPoints()
                                               .WithConsistentDirectionBias(inverseRelationalDirection)
                                               .Take(3);

                var pointToFlyTo = pointsAroundTarget.OfType<Point?>()
                                                     .FirstOrDefault(point => Map.IsWalkable(point!.Value, Subject.Type));

                if (pointToFlyTo.HasValue)
                {
                    Subject.WarpTo(pointToFlyTo);
                    var relationalDirection = Target.DirectionalRelationTo(Subject);
                    Subject.Turn(relationalDirection);

                    if (!Subject.Trackers.TimedEvents.HasActiveEvent("phoenix_pursuit", out _))
                    {
                        Subject.Say("You cannot escape!");
                        Subject.Trackers.TimedEvents.AddEvent("phoenix_pursuit", TimeSpan.FromMinutes(1), true);
                    }
                }

                break;
            }
            case > 1:
            {
                Subject.Pathfind(Target);
                
                break;
            }
            case 1:
            {
                var direction = Target.DirectionalRelationTo(Subject);

                if (Subject.Direction != direction)
                    Subject.Turn(direction);
                
                break;
            }
            case 0:
            {
                Subject.Wander();
                
                break;
            }
        }
        
        ResetAttackTimerIfMoved();
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