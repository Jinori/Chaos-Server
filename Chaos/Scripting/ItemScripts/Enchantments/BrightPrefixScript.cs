using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class BrightPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BrightPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Bright";

        var attributes = new Attributes
        {
            MaximumMp = 150
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Bright"))
            yield return node with { Name = $"Bright {node.Name}" };
    }
}