using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class SoothingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SoothingPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Soothing"))
            subject.DisplayName = $"Soothing {subject.DisplayName}";

        var attributes = new Attributes
        {
            MaximumMp = 500,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Soothing"))
            yield return node with { Name = $"Soothing {node.Name}" };
    }
}