using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class AncientPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public AncientPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Ancient"))
            subject.DisplayName = $"Ancient {subject.DisplayName}";

        var attributes = new Attributes
        {
           MaximumHp = 200,
            MaximumMp = 200,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Ancient"))
            yield return node with { Name = $"Ancient {node.Name}" };
    }
}