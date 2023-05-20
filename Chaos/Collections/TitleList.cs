using Chaos.Common.Collections.Synchronized;
using Chaos.Extensions.Common;

namespace Chaos.Collections;

public sealed class TitleList : SynchronizedList<string>
{
    public TitleList(IEnumerable<string>? items = null)
        : base(items?.Distinct(StringComparer.OrdinalIgnoreCase) ?? Array.Empty<string>()) { }

    /// <inheritdoc />
    public override void Add(string item)
    {
        if (List.ContainsI(item))
            return;

        base.Add(item);
    }

    /// <inheritdoc />
    public override void Insert(int index, string item)
    {
        if (List.ContainsI(item))
            return;

        base.Insert(index, item);
    }

    public override string ToString() => string.Join(Environment.NewLine, this);
}