using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ToughPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ToughPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Tough";

        var attributes = new Attributes
        {
            Str = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Tough"))
            yield return node with { Name = $"Tough {node.Name}" };
    }
}