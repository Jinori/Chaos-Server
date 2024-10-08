using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Microsoft.Extensions.ObjectPool;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Specialized;

public class QuadTree<T> : IEnumerable<T>, IResettable where T: IPoint
{
    private static QuadTreePool Pool;

    // ReSharper disable once StaticMemberInGenericType
    #pragma warning disable CA2211
    public static int Capacity = 32;
    #pragma warning restore CA2211
    protected IRectangle Bounds { get; set; }
    public int Count { get; private set; }
    protected bool Divided { get; set; }
    protected QuadTree<T>?[] Children { get; }
    protected List<T> Items { get; }

    static QuadTree() => Pool = new QuadTreePool(short.MaxValue / 4);

    public QuadTree(IRectangle bounds)
    {
        Items = new List<T>(Capacity);
        Bounds = bounds;
        Children = new QuadTree<T>[4];
        Divided = false;
    }

    public QuadTree()
        : this(new Rectangle()) { }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new QuadTreeEnumerator(this);

        /*if (Count > 0)
        {
            if (Divided)
                for (var i = 0; i < Children.Length; i++)
                {
                    var child = Children[i]!;

                    if (child.Count == 0)
                        continue;

                    foreach (var item in child)
                        yield return item;
                }
            else
                for (var i = 0; i < Items.Count; i++)
                    yield return Items[i];
        }*/
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool TryReset()
    {
        Clear();

        return true;
    }

    public virtual void Clear()
    {
        Items.Clear();
        Count = 0;

        if (Divided)
        {
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i]!;
                Pool.Return(child);
                Children[i] = null;
            }

            Divided = false;
        }
    }

    protected virtual void InnerAdd(T item)
    {
        Items.Add(item);
        Count++;
    }

    protected virtual bool InnerRemove(T item)
    {
        if (Items.Remove(item))
        {
            Count--;

            return true;
        }

        return false;
    }

    public virtual bool Insert(T item)
    {
        if (!Bounds.Contains(item))
            return false;

        if (!Divided && (Count >= Capacity) && Bounds is { Width: > 1, Height: > 1 })
            Subdivide();

        if (Divided)
        {
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i]!;

                if (child.Insert(item))
                {
                    Count++;

                    return true;
                }
            }

            return false;
        }

        InnerAdd(item);

        return true;
    }

    private void Merge()
    {
        for (var i1 = 0; i1 < Children.Length; i1++)
        {
            var child = Children[i1]!;

            for (var i2 = 0; i2 < child.Items.Count; i2++)
            {
                var item = child.Items[i2];

                Items.Add(item);
            }

            Pool.Return(child);
            Children[i1] = null;
        }

        Divided = false;
    }

    public virtual IEnumerable<T> Query(IRectangle bounds)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new QuadTreeRectangleQueryIterator(this, bounds);

        /*if ((Count > 0) && Bounds.Intersects(bounds))
        {
            if (Divided)
                for (var i = 0; i < Children.Length; i++)
                {
                    var child = Children[i]!;

                    if (child.Count == 0)
                        continue;

                    foreach (var item in child.Query(bounds))
                        yield return item;
                }
            else
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];

                    if (bounds.Contains(item))
                        yield return item;
                }
        }*/
    }

    public virtual IEnumerable<T> Query(ICircle circle)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new QuadTreeCircleQueryIterator(this, circle);

        /*if ((Count > 0) && Bounds.Intersects(circle))
        {
            if (Divided)
                for (var i = 0; i < Children.Length; i++)
                {
                    var child = Children[i]!;

                    if (child.Count == 0)
                        continue;

                    foreach (var item in child.Query(circle))
                        yield return item;
                }
            else
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];

                    if (circle.Contains(item))
                        yield return item;
                }
        }*/
    }

    public virtual IEnumerable<T> Query(IPoint point)
    {
        if ((Count > 0) && Bounds.Contains(point))
        {
            if (Divided)
                for (var i = 0; i < Children.Length; i++)
                {
                    var child = Children[i]!;

                    if (child.Count == 0)
                        continue;

                    foreach (var item in child.Query(point))
                        yield return item;
                }
            else
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];

                    if (point.Equals(item))
                        yield return item;
                }
        }
    }

    public virtual bool Remove(T entity)
    {
        if ((Count == 0) || !Bounds.Contains(entity))
            return false;

        if (Divided)
        {
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i]!;

                if (child.Remove(entity))
                {
                    Count--;

                    if (Count < Capacity)
                        Merge();

                    return true;
                }
            }

            return false;
        }

        return InnerRemove(entity);
    }

    public virtual void SetBounds(IRectangle bounds) => Bounds = bounds;

    public static void SetPoolCapacity(int capacity) => Pool = new QuadTreePool(capacity);

    private void Subdivide()
    {
        var x = Bounds.Left;
        var y = Bounds.Top;
        var hw = Bounds.Width / 2;
        var hh = Bounds.Height / 2;

        Children[0] = Pool.Get(
            new Rectangle(
                x,
                y,
                hw,
                hh));

        Children[1] = Pool.Get(
            new Rectangle(
                x + hw,
                y,
                Bounds.Width - hw,
                hh));

        Children[2] = Pool.Get(
            new Rectangle(
                x,
                y + hh,
                hw,
                Bounds.Height - hh));

        Children[3] = Pool.Get(
            new Rectangle(
                x + hw,
                y + hh,
                Bounds.Width - hw,
                Bounds.Height - hh));

        Divided = true;

        for (var i1 = 0; i1 < Items.Count; i1++)
        {
            var item = Items[i1];

            for (var i2 = 0; i2 < Children.Length; i2++)
            {
                var child = Children[i2]!;

                if (child.Insert(item))
                    break;
            }
        }

        Items.Clear();
    }

    #region QuadTreePool
    private sealed class QuadTreePool : DefaultObjectPool<QuadTree<T>>
    {
        public QuadTreePool(int poolCapacity)
            : base(new QuadTreePoolPolicy(), poolCapacity) { }

        public QuadTree<T> Get(Rectangle bounds)
        {
            var ret = Get();
            ret.SetBounds(bounds);

            return ret;
        }
    }

    private sealed class QuadTreePoolPolicy : PooledObjectPolicy<QuadTree<T>>
    {
        public override QuadTree<T> Create() => new(new Rectangle());

        public override bool Return(QuadTree<T> obj)
        {
            obj.Clear();

            return true;
        }
    }
    #endregion

    #region Enumerators
    private struct QuadTreeEnumerator : IEnumerator<T>
    {
        private readonly QuadTree<T> Tree;
        private readonly Stack<QuadTree<T>> Stack;
        private int Index;

        public QuadTreeEnumerator(QuadTree<T> tree)
        {
            Tree = tree;
            Stack = new Stack<QuadTree<T>>();

            Stack.Push(Tree);
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            while (Stack.Count > 0)
            {
                var current = Stack.Peek();

                if (current.Count == 0)
                {
                    Stack.Pop();

                    continue;
                }

                if (current.Divided)
                {
                    Stack.Pop();

                    for (var i = 0; i < current.Children.Length; i++)
                    {
                        var child = current.Children[i]!;

                        if (child.Count == 0)
                            continue;

                        Stack.Push(child);
                    }

                    continue;
                }

                if (Index >= current.Items.Count)
                {
                    Index = 0;
                    Stack.Pop();

                    continue;
                }

                Current = current.Items[Index++];

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Index = 0;
            Stack.Clear();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;
    }

    /// <summary>
    ///     An iterator for querying a <see cref="QuadTree{T}" /> with a rectangle.
    /// </summary>
    /// <remarks>
    ///     Since the query method would need to utilize an iterator block anyway, having the IEnumerator also be an
    ///     IEnumerable saves the runtime from creating an iterator block class
    /// </remarks>
    private struct QuadTreeRectangleQueryIterator : IEnumerator<T>, IEnumerable<T>
    {
        private readonly QuadTree<T> Tree;
        private readonly Stack<QuadTree<T>> Stack;
        private readonly IRectangle Bounds;
        private int Index;

        public QuadTreeRectangleQueryIterator(QuadTree<T> tree, IRectangle bounds)
        {
            Tree = tree;
            Stack = new Stack<QuadTree<T>>();
            Bounds = bounds;

            Stack.Push(Tree);
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            while (Stack.Count > 0)
            {
                var current = Stack.Peek();

                if (current.Count == 0)
                {
                    Stack.Pop();

                    continue;
                }

                if (current.Divided)
                {
                    Stack.Pop();

                    for (var i = 0; i < current.Children.Length; i++)
                    {
                        var child = current.Children[i]!;

                        if ((child.Count == 0) || !child.Bounds.Intersects(Bounds))
                            continue;

                        Stack.Push(child);
                    }

                    continue;
                }

                if (Index >= current.Items.Count)
                {
                    Index = 0;
                    Stack.Pop();

                    continue;
                }

                var item = current.Items[Index++];

                if (Bounds.Contains(item))
                {
                    Current = item;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Index = 0;
            Stack.Clear();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    ///     An iterator for querying a <see cref="QuadTree{T}" /> with a circle.
    /// </summary>
    /// <remarks>
    ///     Since the query method would need to utilize an iterator block anyway, having the IEnumerator also be an
    ///     IEnumerable saves the runtime from creating an iterator block class
    /// </remarks>
    private struct QuadTreeCircleQueryIterator : IEnumerator<T>, IEnumerable<T>
    {
        private readonly QuadTree<T> Tree;
        private readonly Stack<QuadTree<T>> Stack;
        private readonly ICircle Bounds;
        private int Index;

        public QuadTreeCircleQueryIterator(QuadTree<T> tree, ICircle bounds)
        {
            Tree = tree;
            Stack = new Stack<QuadTree<T>>();
            Bounds = bounds;

            Stack.Push(Tree);
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            while (Stack.Count > 0)
            {
                var current = Stack.Peek();

                if (current.Count == 0)
                {
                    Stack.Pop();

                    continue;
                }

                if (current.Divided)
                {
                    Stack.Pop();

                    for (var i = 0; i < current.Children.Length; i++)
                    {
                        var child = current.Children[i]!;

                        if ((child.Count == 0) || !child.Bounds.Intersects(Bounds, DistanceType.Manhattan))
                            continue;

                        Stack.Push(child);
                    }

                    continue;
                }

                if (Index >= current.Items.Count)
                {
                    Index = 0;
                    Stack.Pop();

                    continue;
                }

                var item = current.Items[Index++];

                if (Bounds.Contains(item, DistanceType.Manhattan))
                {
                    Current = item;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Index = 0;
            Stack.Clear();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    #endregion
}