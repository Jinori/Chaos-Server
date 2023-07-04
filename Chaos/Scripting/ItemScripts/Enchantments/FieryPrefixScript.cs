using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class FieryPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public FieryPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Fiery"))
            subject.DisplayName = $"Fiery {subject.DisplayName}";

        var attributes = new Attributes
        {
            AtkSpeedPct = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Fiery"))
            yield return node with { Name = $"Fiery {node.Name}" };
    }
}