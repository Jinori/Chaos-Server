using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PrecisionPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PrecisionPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Precision";

        var attributes = new Attributes
        {
            Hit = 4
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Precision"))
            yield return node with { Name = $"Precision {node.Name}" };
    }
}