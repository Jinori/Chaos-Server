using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class PrecisionPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PrecisionPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Precision"))
            subject.DisplayName = $"Precision {subject.DisplayName}";

        var attributes = new Attributes
        {
            Hit = 5,
            Dmg = -2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Precision"))
            yield return node with { Name = $"Precision {node.Name}" };
    }
}