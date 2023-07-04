using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class PowerfulPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PowerfulPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Powerful"))
            subject.DisplayName = $"Powerful {subject.DisplayName}";

        var attributes = new Attributes
        {
            Dmg = 7,
            Con = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Powerful"))
            yield return node with { Name = $"Powerful {node.Name}" };
    }
}