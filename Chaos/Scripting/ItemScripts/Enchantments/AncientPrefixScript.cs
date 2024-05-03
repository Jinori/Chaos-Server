using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class AncientPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public AncientPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Ancient";

        var attributes = new Attributes
        {
            MaximumHp = 300,
            MaximumMp = 150
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Ancient"))
            yield return node with { Name = $"Ancient {node.Name}" };
    }
}