using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class LuckyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public LuckyPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Lucky"))
            subject.DisplayName = $"Lucky {subject.DisplayName}";

        var attributes = new Attributes
        {
           Hit = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Lucky"))
            yield return node with { Name = $"Lucky {node.Name}" };
    }
}