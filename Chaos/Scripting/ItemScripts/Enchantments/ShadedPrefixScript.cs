using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ShadedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ShadedPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Shaded";

        var attributes = new Attributes
        {
            AtkSpeedPct = 4,
            Dmg = 3
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Shaded"))
            yield return node with { Name = $"Shaded {node.Name}" };
    }
}