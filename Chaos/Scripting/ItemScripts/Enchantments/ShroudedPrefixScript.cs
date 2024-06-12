using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ShroudedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ShroudedPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Shrouded";

        var attributes = new Attributes
        {
            Dmg = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Shrouded"))
            yield return node with { Name = $"Shrouded {node.Name}" };
    }
}