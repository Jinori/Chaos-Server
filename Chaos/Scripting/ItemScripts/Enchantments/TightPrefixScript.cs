using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class TightPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public TightPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Tight";

        var attributes = new Attributes
        {
            AtkSpeedPct = 4,
            MaximumHp = -80
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Tight"))
            yield return node with { Name = $"Tight {node.Name}" };
    }
}