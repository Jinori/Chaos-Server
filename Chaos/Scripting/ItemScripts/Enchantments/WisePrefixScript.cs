using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class WisePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public WisePrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Wise"))
            subject.DisplayName = $"Wise {subject.DisplayName}";

        var attributes = new Attributes
        {
           Wis = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Wise"))
            yield return node with { Name = $"Wise {node.Name}" };
    }
}