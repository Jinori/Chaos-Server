using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class ShroudedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ShroudedPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Shrouded"))
            subject.DisplayName = $"Shrouded {subject.DisplayName}";

        var attributes = new Attributes
        {
            Dmg = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Shrouded"))
            yield return node with { Name = $"Shrouded {node.Name}" };
    }
}