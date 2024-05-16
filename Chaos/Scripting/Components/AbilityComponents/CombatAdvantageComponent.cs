using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class CombatAdvantageComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ICombatAdvantageComponentOptions>();

        if (options.DistanceToAmbush is null)
            return;

        var endPoint = context.Source.DirectionalOffset(context.Source.Direction, options.DistanceToAmbush.Value);

        var points = context.Source
                            .GetDirectPath(endPoint)
                            .Skip(1);

        foreach (var point in points)
        {
            if (context.TargetMap.IsWall(point) || context.TargetMap.IsBlockingReactor(point))
                return;

            var entity = context.TargetMap
                                .GetEntitiesAtPoint<Creature>(point)
                                .TopOrDefault();

            if (entity != null)
            {
                //get the direction that vectors behind the target relative to the source
                var behindTargetDirection = entity.DirectionalRelationTo(context.SourcePoint);

                //for each direction around the target, starting with the direction behind the target
                foreach (var direction in behindTargetDirection.AsEnumerable())
                {
                    //get the point in that direction
                    var destinationPoint = entity.DirectionalOffset(direction);

                    //if that point is not walkable or is a reactor, continue
                    if (!context.TargetMap.IsWalkable(destinationPoint, context.Source.Type)
                        || context.TargetMap.IsBlockingReactor(destinationPoint))
                        continue;

                    //if it is walkable, warp to that point and turn to face the target
                    context.Source.WarpTo(destinationPoint);
                    var newDirection = entity.DirectionalRelationTo(context.Source);
                    context.Source.Turn(newDirection);

                    return;
                }
            }
        }
    }

    public interface ICombatAdvantageComponentOptions
    {
        int? DistanceToAmbush { get; init; }
    }
}