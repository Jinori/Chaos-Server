using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CthonicDemise.Leaders.MonkLeader;

public sealed class MonkBossMoveToTargetScript(Monster subject) : MonsterScriptBase(subject)
{
    private void ResetAttackTimerIfMoved()
    {
        var now = DateTime.UtcNow;
        var lastWalk = Subject.Trackers.LastWalk;
        var lastTurn = Subject.Trackers.LastTurn;

        var walkedRecently = lastWalk.HasValue
                             && (now.Subtract(lastWalk.Value)
                                    .TotalMilliseconds
                                 < Subject.Template.MoveIntervalMs);

        var turnedRecently = lastTurn.HasValue
                             && (now.Subtract(lastTurn.Value)
                                    .TotalMilliseconds
                                 < Subject.Template.MoveIntervalMs);

        if (walkedRecently || turnedRecently)
        {
            Subject.WanderTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        var point = Target.GenerateCardinalPoints()
                          .OfType<IPoint>()
                          .FirstOrDefault(x => Subject.MapInstance.IsWalkable(x, Subject.Type));

        //Remove aggro if we can't get to the target so we have a chance to attack someone else
        if (point is null
            && (Map.GetEntities<Aisling>()
                   .Count()
                > 1))
        {
            AggroList.Remove(Target.Id, out _);
            Target = null;

            return;
        }

        switch (distance)
        {
            case >= 4:
                var safeSpot = Target.GenerateCardinalPoints()
                                     .OfType<IPoint>()
                                     .FirstOrDefault(x => Subject.MapInstance.IsWalkable(x, Subject.Type));

                if (safeSpot is null)
                {
                    AggroList.Remove(Target.Id, out _);

                    Target ??= Map.GetEntitiesWithinRange<Aisling>(Subject, 12)
                                  .ThatAreVisibleTo(Subject)
                                  .Where(
                                      obj => !obj.Equals(Subject)
                                             && obj.IsAlive
                                             && Subject.ApproachTime.TryGetValue(obj, out var time)
                                             && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
                                  .ClosestOrDefault(Subject);

                    return;
                }

                Subject.TraverseMap(Target.MapInstance, safeSpot);

                break;

            case > 1:
                Subject.Pathfind(Target);

                break;
            case 1:
            {
                var direction = Target.DirectionalRelationTo(Subject);
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
}