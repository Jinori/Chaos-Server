#region
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.IRectangle" />
/// </summary>
public static class RectangleExtensions
{
    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> contains another
    ///     <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">
    ///     The possibly outer rectangle
    /// </param>
    /// <param name="other">
    ///     The possible inner rectangle
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if <paramref name="rect" /> fully encompasses <paramref name="other" />, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this IRectangle rect, IRectangle other)
    {
        ArgumentNullException.ThrowIfNull(rect);

        ArgumentNullException.ThrowIfNull(other);

        return (rect.Bottom >= other.Bottom) && (rect.Left <= other.Left) && (rect.Right >= other.Right) && (rect.Top <= other.Top);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> contains an
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="rect">
    ///     The rectangle to check
    /// </param>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if <paramref name="point" /> is inside of the <paramref name="rect" />, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    [OverloadResolutionPriority(1), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this IRectangle rect, Point point)
    {
        ArgumentNullException.ThrowIfNull(rect);

        return (rect.Left <= point.X) && (rect.Right >= point.X) && (rect.Top <= point.Y) && (rect.Bottom >= point.Y);
    }

    /// <inheritdoc cref="Contains(IRectangle, Point)" />
    public static bool Contains(this IRectangle rect, IPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);

        return rect.Contains(Point.From(point));
    }

    /// <summary>
    ///     Given a start and end point, generates a maze inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">
    ///     The rect that represents the bounds of the maze
    /// </param>
    /// <param name="start">
    ///     The start point of the maze
    /// </param>
    /// <param name="end">
    ///     The end point of the maze
    /// </param>
    /// <returns>
    /// </returns>
    [OverloadResolutionPriority(1)]
    public static IEnumerable<Point> GenerateMaze(this IRectangle rect, Point start, Point end)
    {
        //neighbor pattern
        List<(int, int)> pattern =
        [
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0)
        ];

        var height = rect.Height;
        var width = rect.Width;
        var startNode = UnOffsetCoordinates(rect, start);
        var endNode = UnOffsetCoordinates(rect, end);
        var maze = new bool[width, height];
        var discoveryQueue = new Stack<Point>();

        var mazeRect = new Rectangle(
            0,
            0,
            width,
            height);

        //initialize maze full of walls
        for (var x = 0; x < maze.GetLength(0); x++)
            for (var y = 0; y < maze.GetLength(1); y++)
                maze[x, y] = true;

        //start wtih startNode
        //startNode is not a wall
        discoveryQueue.Push(startNode);
        maze[startNode.X, startNode.Y] = false;

        //carve out the maze
        while (discoveryQueue.Count > 0)
        {
            //get current node
            var current = discoveryQueue.Peek();
            var carved = false;

            //shuffle neighbor pattern so we get a random direction
            ShuffleInPlace(pattern);

            //for each direction in the pattern
            foreach ((var dx, var dy) in pattern)
            {
                //get a point 2 spaces in the direction
                var target = new Point(current.X + dx * 2, current.Y + dy * 2);

                //if the target is out of bounds or already carved, skip
                if (!mazeRect.Contains(target) || !maze[target.X, target.Y])
                    continue;

                //carve out the wall between the current node and the target
                //carve out the target node
                maze[current.X + dx, current.Y + dy] = false;
                maze[target.X, target.Y] = false;

                //push the target node onto the stack
                discoveryQueue.Push(target);
                carved = true;

                //don't look at any more of these neighbors
                break;
            }

            //since we carved, pop the node we peeked
            if (!carved)
                discoveryQueue.Pop();
        }

        //end node is not a wall
        maze[endNode.X, endNode.Y] = false;

        //yield all walls in the maze
        for (var x = 0; x < maze.GetLength(0); x++)
            for (var y = 0; y < maze.GetLength(1); y++)
                if (maze[x, y])
                    yield return OffsetCoordinates(rect, new Point(x, y));

        yield break;

        static void ShuffleInPlace<T>(IList<T> arr)
        {
            for (var i = arr.Count - 1; i > 0; i--)
            {
                var j = Random.Shared.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        static Point OffsetCoordinates(IRectangle rect, IPoint point) => new(point.X + rect.Left, point.Y + rect.Top);

        static Point UnOffsetCoordinates(IRectangle rect, IPoint point) => new(point.X - rect.Left, point.Y - rect.Top);
    }

    /// <inheritdoc cref="GenerateMaze(IRectangle, Point, Point)" />
    public static IEnumerable<Point> GenerateMaze(this IRectangle rect, IPoint start, IPoint end)
    {
        ArgumentNullException.ThrowIfNull(start);

        ArgumentNullException.ThrowIfNull(end);

        return rect.GenerateMaze(Point.From(start), Point.From(end));
    }

    /// <summary>
    ///     Lazily generates points along the outline of the rectangle. The points will be in the order the vertices are
    ///     listed.
    /// </summary>
    public static IEnumerable<Point> GetOutline(this IRectangle rect)
    {
        var vertices = rect.Vertices;

        for (var i = 0; i < (vertices.Count - 1); i++)
        {
            var current = vertices[i];
            var next = vertices[i + 1];

            //skip the last point so the vertices are not included twice
            foreach (var point in current.GetDirectPath(next)
                                         .SkipLast(1))
                yield return point;
        }

        foreach (var point in vertices[^1]
                              .GetDirectPath(vertices[0])
                              .SkipLast(1))
            yield return point;
    }

    /// <summary>
    ///     Lazily generates all points inside of the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">
    ///     The rectangle togenerate points for
    /// </param>
    public static IEnumerable<Point> GetPoints(this IRectangle rect)
    {
        ArgumentNullException.ThrowIfNull(rect);

        for (var x = rect.Left; x <= rect.Right; x++)
            for (var y = rect.Top; y <= rect.Bottom; y++)
                yield return new Point(x, y);
    }

    /// <summary>
    ///     Generates a random point inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">
    ///     The rect to use as bounds
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point GetRandomPoint(this IRectangle rect)
        => new(rect.Left + Random.Shared.Next(rect.Width), rect.Top + Random.Shared.Next(rect.Height));

    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> intersects another
    ///     <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">
    ///     A rectangle
    /// </param>
    /// <param name="other">
    ///     Another rectangle
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the rectangles intersect at any point or if either rect fully contains the other, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(this IRectangle rect, IRectangle other)
    {
        ArgumentNullException.ThrowIfNull(rect);

        ArgumentNullException.ThrowIfNull(other);

        return !((rect.Bottom < other.Top) || (rect.Left > other.Right) || (rect.Right < other.Left) || (rect.Top > other.Bottom));
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> intersects a
    /// </summary>
    /// <param name="rect">
    ///     A rectangle
    /// </param>
    /// <param name="circle">
    ///     A circle
    /// </param>
    /// <param name="distanceType">
    ///     The type of distance check to use when measuring distance
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the rectangle intersects the circle at any point, or if either fully contains the other, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(this IRectangle rect, ICircle circle, DistanceType distanceType = DistanceType.Euclidean)
    {
        ArgumentNullException.ThrowIfNull(rect);

        ArgumentNullException.ThrowIfNull(circle);

        var closestX = Math.Clamp(circle.Center.X, rect.Left, rect.Right);
        var closestY = Math.Clamp(circle.Center.Y, rect.Top, rect.Bottom);

        return distanceType switch
        {
            DistanceType.Manhattan => new Point(closestX, closestY).ManhattanDistanceFrom(Point.From(circle.Center)) <= circle.Radius,
            DistanceType.Euclidean => new Point(closestX, closestY).EuclideanDistanceFrom(Point.From(circle.Center)) <= circle.Radius,
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <summary>
    ///     Generates a random point inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">
    ///     The rect to use as bounds
    /// </param>
    /// <param name="predicate">
    ///     A predicate the point must match
    /// </param>
    /// <param name="point">
    ///     A random point that matches the given predicate
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a random point was found that matches the predicate, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool TryGetRandomPoint(this IRectangle rect, Func<Point, bool> predicate, [NotNullWhen(true)] out Point? point)
    {
        point = null;

        if (!rect.GetPoints()
                 .Any(predicate))
            return false;

        while (true)
        {
            var randomPoint = rect.GetRandomPoint();

            if (predicate(randomPoint))
            {
                point = randomPoint;

                return true;
            }
        }
    }
}