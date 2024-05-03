using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class LuckyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public LuckyPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Lucky";

        var attributes = new Attributes
        {
            Hit = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Lucky"))
            yield return node with { Name = $"Lucky {node.Name}" };
    }
}