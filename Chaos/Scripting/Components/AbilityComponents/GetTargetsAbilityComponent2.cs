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

public struct GetTargetsAbilityComponent2<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetTargetsComponentOptions>();
        var map = context.TargetMap;
        var aoeOptions = CreateOptions(context, options);

        var targetPoints = options.Shape2
                                  .ResolvePoints(aoeOptions)
                                  .ToList();

        if (options.StopOnWalls2)
            targetPoints = targetPoints.FilterByLineOfSight(context.TargetPoint, context.TargetMap)
                                       .ToList();

        var targetEntities = map.GetEntitiesAtPoints<TEntity>(targetPoints)
                                .WithFilter(context.Source, options.Filter2)
                                .ToList();
        
        if (context.SourceAisling is { IsAdmin: false })
            targetEntities = targetEntities.ExcludeHiddenGms()
                                           .ToList();

        if (options.StopOnFirstHit2)
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

        if (options.SingleTarget2 && (targetEntities.Count > 1))
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

        return !options.MustHaveTargets2 || (targetEntities.Count != 0);
    }

    private AoeShapeOptions CreateOptions(ActivationContext context, IGetTargetsComponentOptions options)
    {
        var direction = context.SnapshotTargetDirection ?? context.SnapshotTargetPoint.DirectionalRelationTo(context.SnapshotSourcePoint);

        if (direction == Direction.Invalid)
            direction = context.SnapshotSourceDirection;

        return new AoeShapeOptions
        {
            Direction = direction,
            ExclusionRange = options.ExclusionRange2,
            Range = options.Range2,
            Source = context.SnapshotTargetPoint
        };
    }

    public interface IGetTargetsComponentOptions
    {
        int? ExclusionRange2 { get; init; }
        TargetFilter Filter2 { get; init; }
        bool MustHaveTargets2 { get; init; }
        int Range2 { get; init; }
        AoeShape Shape2 { get; init; }
        bool SingleTarget2 { get; init; }
        bool StopOnFirstHit2 { get; init; }
        bool StopOnWalls2 { get; init; }
    }
}