using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.MythicBosses.MantisBoss.Mantis1;

public sealed class EMythicMantis1MoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public EMythicMantis1MoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>().Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        if ((distance != 1) && (distance <= 4))
            Subject.Pathfind(Target);
        else if (distance >= 4)
        {
            var points = Subject.GetDirectPath(Target)
                                .Skip(1);

            foreach (var point in points)
            {
                if (Subject.MapInstance.IsWall(point) || Subject.MapInstance.IsBlockingReactor(point))
                    return;

                var entity = Subject.MapInstance.GetEntitiesAtPoints<Aisling>(point)
                                    .TopOrDefault();

                if (entity != null)
                {
                    //get the direction that vectors behind the target relative to the source
                    var behindTargetDirection = entity.DirectionalRelationTo(Subject);

                    //for each direction around the target, starting with the direction behind the target
                    foreach (var direction in behindTargetDirection.AsEnumerable())
                    {
                        //get the point in that direction
                        var destinationPoint = entity.DirectionalOffset(direction);

                        //if that point is not walkable or is a reactor, continue
                        if (!Subject.MapInstance.IsWalkable(destinationPoint, Subject.Type)
                            || Subject.MapInstance.IsBlockingReactor(destinationPoint))
                            continue;

                        //if it is walkable, warp to that point and turn to face the target
                        Subject.WarpTo(destinationPoint);
                        var newDirection = entity.DirectionalRelationTo(Subject);
                        Subject.Turn(newDirection);

                        return;
                    }
                }
            }
        }
        else
        {
            var direction = Target.DirectionalRelationTo(Subject);
            Subject.Turn(direction);
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}