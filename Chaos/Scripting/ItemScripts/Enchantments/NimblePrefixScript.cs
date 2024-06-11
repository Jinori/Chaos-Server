using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class NimblePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public NimblePrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Nimble";

        var attributes = new Attributes
        {
            Dex = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Nimble"))
            yield return node with { Name = $"Nimble {node.Name}" };
    }
}