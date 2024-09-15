using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SpiritedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SpiritedPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Spirited";

        var attributes = new Attributes
        {
            MaximumMp = 500
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Spirited"))
            yield return node with { Name = $"Spirited {node.Name}" };
    }
}