using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class FocusedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public FocusedPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Focused";

        var attributes = new Attributes
        {
            Hit = 1,
            AtkSpeedPct = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Focused"))
            yield return node with { Name = $"Focused {node.Name}" };
    }
}