using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class MinorPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MinorPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Minor"))
            subject.DisplayName = $"Minor {subject.DisplayName}";

        var attributes = new Attributes
        {
            MaximumHp = -50,
            Dmg = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Minor"))
            yield return node with { Name = $"Minor {node.Name}" };
    }
}