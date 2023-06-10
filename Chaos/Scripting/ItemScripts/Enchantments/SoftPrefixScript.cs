using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class SoftPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SoftPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Soft"))
            subject.DisplayName = $"Soft {subject.DisplayName}";

        var attributes = new Attributes
        {
           MagicResistance = 10,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Soft"))
            yield return node with { Name = $"Soft {node.Name}" };
    }
}