using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class HalePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HalePrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Hale"))
            subject.DisplayName = $"Hale {subject.DisplayName}";

        var attributes = new Attributes
        {
           AtkSpeedPct = 5,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Hale"))
            yield return node with { Name = $"Hale {node.Name}" };
    }
}