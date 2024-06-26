using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Pathfinding.Abstractions;

/// <summary>
///     Defines a pattern for a service that performs pathfinding on any number of grids
/// </summary>
public interface IPathfindingService
{
    /// <summary>
    ///     Finds a path from the start to the end with options to ignorewalls or path around certain creatures
    /// </summary>
    /// <param name="gridKey">
    ///     The key of the grid to perform pathfinding on
    /// </param>
    /// <param name="start">
    ///     The point to start pathfinding from
    /// </param>
    /// <param name="end">
    ///     The point to pathfind to
    /// </param>
    /// <param name="pathOptions">
    ///     Path generation options
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk to move to the next point in the path
    /// </returns>
    /// <returns>
    /// </returns>
    Stack<IPoint> FindPath(
        string gridKey,
        IPoint start,
        IPoint end,
        IPathOptions? pathOptions = null);

    /// <summary>
    ///     Finds a valid direction to wander
    /// </summary>
    /// <param name="key">
    ///     The key of the grid to perform pathfinding on
    /// </param>
    /// <param name="start">
    ///     The current point
    /// </param>
    /// <param name="pathOptions">
    ///     Path generation options
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk
    /// </returns>
    Direction FindRandomDirection(string key, IPoint start, IPathOptions? pathOptions = null);

    /// <summary>
    ///     Finds a direction to walk towards the end point. No path is calculated.
    /// </summary>
    /// <param name="gridKey">
    ///     The key of the grid to find a path on
    /// </param>
    /// <param name="start">
    ///     The point to start from
    /// </param>
    /// <param name="end">
    ///     The point to walk towards
    /// </param>
    /// <param name="pathOptions">
    ///     Path generation options
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to move
    /// </returns>
    public Direction FindSimpleDirection(
        string gridKey,
        IPoint start,
        IPoint end,
        IPathOptions? pathOptions = null);

    /// <summary>
    ///     Registers a grid with the pathfinding service
    /// </summary>
    /// <param name="key">
    ///     The key used to locate the grid
    /// </param>
    /// <param name="gridDetails">
    ///     Required details about the grid
    /// </param>
    void RegisterGrid(string key, IGridDetails gridDetails);
}