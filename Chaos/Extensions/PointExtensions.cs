#region
using System.Runtime.CompilerServices;
using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Extensions;

public static class PointExtensions
{
    [OverloadResolutionPriority(1)]
    public static IEnumerable<Point> FilterByLineOfSight(this IEnumerable<Point> points, Point origin, MapInstance mapInstance)
    {
        ArgumentNullException.ThrowIfNull(points);

        ArgumentNullException.ThrowIfNull(mapInstance);

        return points.Where(
            point => !mapInstance.IsWall(point)
                     && !origin.RayTraceTo(point)
                               .Any(mapInstance.IsWall));
    }

    public static IEnumerable<IPoint> FilterByLineOfSight(this IEnumerable<IPoint> points, IPoint origin, MapInstance mapInstance)
    {
        ArgumentNullException.ThrowIfNull(points);

        ArgumentNullException.ThrowIfNull(origin);

        ArgumentNullException.ThrowIfNull(mapInstance);

        return points.Where(
            point => !mapInstance.IsWall(point)
                     && !origin.RayTraceTo(point)
                               .Any(mapInstance.IsWall));
    }

    [OverloadResolutionPriority(1), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this Point point, Point other, int distance = 15) => point.ManhattanDistanceFrom(other) <= distance;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this IPoint point, IPoint other, int distance = 15)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        return WithinRange(Point.From(point), Point.From(other), distance);
    }
}