using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class HalePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HalePrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Hale";

        var attributes = new Attributes
        {
            Con = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Hale"))
            yield return node with { Name = $"Hale {node.Name}" };
    }
}