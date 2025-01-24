#region
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public class GetMultistrikeTargetsAbilityComponent<TEntity> : IConditionalComponent where TEntity : Creature
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IGetMultistrikeTargetsOptions>();
        var source = context.Source;
        var map = context.SourceMap;
        
        var targetEntities = map.GetEntitiesWithinRange<TEntity>(source, options.Range)
                                .Where(target => IsValidTarget(source, target, options))
                                .OrderBy(target => target.ManhattanDistanceFrom(source))
                                .Take(5) // Limit to 5 targets
                                .ToList();
        
        // Extract points assuming TEntity implements IMapEntity with X and Y properties
        var targetPoints = targetEntities
            .Select(target => new Point(target.X, target.Y))
            .ToList();

        // Save points and targets into ComponentVars
        vars.SetPoints(targetPoints);
        vars.SetTargets(targetEntities);

        // Return true if at least one target is found, or if no targets are required
        return !options.MustHaveTargets || (targetEntities.Count != 0);
    }
    
    private bool IsValidTarget(Creature source, TEntity target, IGetMultistrikeTargetsOptions options)
    {
        // Ensure TEntity can be cast to Creature
        Creature creatureTarget = target;

        return creatureTarget.IsHostileTo(source) &&
               !creatureTarget.IsDead &&
               ((options.Filter & TargetFilter.HostileOnly) != 0) &&
               (!options.StopOnWalls || IsSameSideOrNoWallBetween(source, creatureTarget));
    }

    private bool IsSameSideOrNoWallBetween(Creature source, Creature target)
    {
        var path = source.GetDirectPath(target);

        // Ensure no walls exist along the path
        return !path.Any(point => source.MapInstance.IsWall(point));
    }

    public interface IGetMultistrikeTargetsOptions
    {
        int Range { get; init; }
        TargetFilter Filter { get; init; }
        bool MustHaveTargets { get; init; }
        bool StopOnWalls { get; init; }
    }
}