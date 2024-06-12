using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class DarkenedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public DarkenedPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Darkened";

        var attributes = new Attributes
        {
            SpellDamagePct = 2,
            Ac = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Darkened"))
            yield return node with { Name = $"Darkened {node.Name}" };
    }
}