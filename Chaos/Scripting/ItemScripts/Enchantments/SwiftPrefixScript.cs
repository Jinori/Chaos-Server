using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SwiftPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SwiftPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Swift";

        var attributes = new Attributes
        {
            MaximumHp = 80
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Swift"))
            yield return node with { Name = $"Swift {node.Name}" };
    }
}