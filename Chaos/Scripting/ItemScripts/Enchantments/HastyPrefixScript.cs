using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class HastyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HastyPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Hasty"))
            subject.DisplayName = $"Hasty {subject.DisplayName}";

        var attributes = new Attributes
        {
            AtkSpeedPct = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Hasty"))
            yield return node with { Name = $"Hasty {node.Name}" };
    }
}