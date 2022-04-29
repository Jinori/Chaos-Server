using System.Collections.Generic;
using System.Linq;
using Chaos.Core.Geometry;
using Chaos.PanelObjects;
using Chaos.WorldObjects;

namespace Chaos.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<GroundItem> AsGroundItems(this IEnumerable<Item> items, Point point) => items
        .Select(item => item.ToGroundItem(point));

    public static Item ToSingleStack(this IEnumerable<Item> items) =>
        items.Aggregate((item1, item2) =>
        {
            item1.Count += item2.Count;

            return item1;
        });

    public static IEnumerable<Item> FixStacks(this IEnumerable<Item> items) => items.SelectMany(item => item.FixStacks());
}