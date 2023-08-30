using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SoothingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SoothingPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Soothing";

        var attributes = new Attributes
        {
            MaximumMp = 500
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Soothing"))
            yield return node with { Name = $"Soothing {node.Name}" };
    }
}