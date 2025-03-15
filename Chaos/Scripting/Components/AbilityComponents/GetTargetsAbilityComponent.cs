#region
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct GetTargetsAbilityComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetTargetsComponentOptions>();
        var map = context.TargetMap;
        var aoeOptions = CreateOptions(context, options);

        var targetPoints = options.Shape
                                  .ResolvePoints(aoeOptions)
                                  .ToList();

        if (options.StopOnWalls)
            targetPoints = targetPoints.FilterByLineOfSight(context.TargetPoint, context.TargetMap)
                                       .ToList();

        var targetEntities = map.GetEntitiesAtPoints<TEntity>(targetPoints)
                                .WithFilter(context.Source, options.Filter)
                                .ToList();

        if (options.StopOnFirstHit)
            targetEntities = targetEntities.Where(
                                               x =>
                                               {
                                                   var points = context.TargetPoint
                                                                       .RayTraceTo(x)
                                                                       .Skip(1)
                                                                       .SkipLast(1);
                                                   var entities = targetEntities;

                                                   //there are no entities between the source and the target
                                                   return points.All(
                                                       point => !entities.Any(e => PointEqualityComparer.Instance.Equals(e, point)));
                                               })
                                           .ToList();

        if (options.SingleTarget && (targetEntities.Count > 1))
        {
            if (context.TargetCreature is TEntity entity && targetEntities.Contains(entity))
                targetEntities = [entity];
            else
                targetEntities =
                [
                    targetEntities.OrderBy(e => e.Creation)
                                  .First()
                ];
        }

        vars.SetPoints(targetPoints);
        vars.SetTargets(targetEntities);

        return !options.MustHaveTargets || (targetEntities.Count != 0);
    }

    private AoeShapeOptions CreateOptions(ActivationContext context, IGetTargetsComponentOptions options)
    {
        var direction = context.SnapshotTargetDirection ?? context.SnapshotTargetPoint.DirectionalRelationTo(context.SnapshotSourcePoint);

        if (direction == Direction.Invalid)
            direction = context.SnapshotSourceDirection;

        return new AoeShapeOptions
        {
            Direction = direction,
            ExclusionRange = options.ExclusionRange,
            Range = options.Range,
            Source = context.SnapshotTargetPoint
        };
    }

    public interface IGetTargetsComponentOptions
    {
        int? ExclusionRange { get; init; }
        TargetFilter Filter { get; init; }
        bool MustHaveTargets { get; init; }
        int Range { get; init; }
        AoeShape Shape { get; init; }
        bool SingleTarget { get; init; }
        bool StopOnFirstHit { get; init; }
        bool StopOnWalls { get; init; }
    }
}