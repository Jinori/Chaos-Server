using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ResilientPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ResilientPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Resilient";

        var attributes = new Attributes
        {
            MagicResistance = 2,
            Ac = -1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Resilient"))
            yield return node with { Name = $"Resilient {node.Name}" };
    }
}