using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class AiryPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public AiryPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Airy";

        var attributes = new Attributes
        {
            Hit = 3
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Airy"))
            yield return node with { Name = $"Airy {node.Name}" };
    }
}