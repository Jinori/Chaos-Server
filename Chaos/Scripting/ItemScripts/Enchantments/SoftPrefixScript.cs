using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SoftPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SoftPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Soft";

        var attributes = new Attributes
        {
            MagicResistance = 3,
            Hit = -1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Soft"))
            yield return node with { Name = $"Soft {node.Name}" };
    }
}