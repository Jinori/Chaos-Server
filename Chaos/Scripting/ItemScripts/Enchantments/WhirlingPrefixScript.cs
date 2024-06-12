using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class WhirlingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public WhirlingPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Whirling";

        var attributes = new Attributes
        {
            MagicResistance = 5,
            Hit = -2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Whirling"))
            yield return node with { Name = $"Whirling {node.Name}" };
    }
}