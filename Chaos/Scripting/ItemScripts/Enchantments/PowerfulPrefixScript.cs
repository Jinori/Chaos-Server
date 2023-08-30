using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PowerfulPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PowerfulPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Powerful";

        var attributes = new Attributes
        {
            Dmg = 7,
            Con = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Powerful"))
            yield return node with { Name = $"Powerful {node.Name}" };
    }
}