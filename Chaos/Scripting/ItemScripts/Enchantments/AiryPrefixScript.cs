using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class AiryPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public AiryPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Airy"))
            subject.DisplayName = $"Airy {subject.DisplayName}";

        var attributes = new Attributes
        {
            Hit = 5,
            Int = -2
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