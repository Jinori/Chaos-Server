using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class TightPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public TightPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Tight"))
            subject.DisplayName = $"Tight {subject.DisplayName}";

        var attributes = new Attributes
        {
            AtkSpeedPct = 7,
            Dex = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Tight"))
            yield return node with { Name = $"Tight {node.Name}" };
    }
}