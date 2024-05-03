using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class MinorPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MinorPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Minor";

        var attributes = new Attributes
        {
            Dmg = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Minor"))
            yield return node with { Name = $"Minor {node.Name}" };
    }
}