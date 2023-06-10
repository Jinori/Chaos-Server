using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class NimblePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public NimblePrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Nimble"))
            subject.DisplayName = $"Nimble {subject.DisplayName}";

        var attributes = new Attributes
        {
           Dex = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Nimble"))
            yield return node with { Name = $"Nimble {node.Name}" };
    }
}