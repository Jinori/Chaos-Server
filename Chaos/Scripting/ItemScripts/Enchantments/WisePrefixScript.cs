using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class WisePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public WisePrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Wise";

        var attributes = new Attributes
        {
            Wis = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Wise"))
            yield return node with { Name = $"Wise {node.Name}" };
    }
}