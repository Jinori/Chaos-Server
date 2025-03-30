using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.ItemScripts.Goddess_Enchants;

public sealed class TheseleneSuffixScript : ItemScriptBase, IEnchantmentScript
{
    private readonly RandomizedIntervalTimer ShadowTimer = new(TimeSpan.FromSeconds(10), 30);
    private Aisling? Owner { get; set; }

    /// <inheritdoc />
    public TheseleneSuffixScript(Item subject)
        : base(subject)
        => Subject.Suffix = "of Shadows";

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.EndsWithI("of Shadows"))
            yield return node with
            {
                Name = $"{node.Name} of Shadows"
            };
    }

    /// <inheritdoc />
    public override void OnEquipped(Aisling aisling)
    {
        base.OnEquipped(aisling);

        Owner = aisling;
    }

    /// <inheritdoc />
    public override void OnUnEquipped(Aisling aisling)
    {
        base.OnUnEquipped(aisling);

        Owner = null;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ShadowTimer.Update(delta);

        if (!ShadowTimer.IntervalElapsed)
            return;

        if (Owner is null)
            return;

        var mobs = Owner.MapInstance
                        .GetEntitiesWithinRange<Monster>(Owner, 8)
                        .Where(x => x.AggroList.ContainsKey(Owner.Id))
                        .ToList();

        foreach (var mob in mobs)

            // Reduce aggro by 10%
            if (mob.AggroList.TryGetValue(Owner.Id, out var currentAggro))
            {
                var newAggro = (int)(currentAggro * 0.9);
                mob.AggroList.TryUpdate(Owner.Id, newAggro, currentAggro);
            }
    }
}