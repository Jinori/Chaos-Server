using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class ToughPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ToughPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Tough"))
            subject.DisplayName = $"Tough {subject.DisplayName}";

        var attributes = new Attributes
        {
           Str = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Tough"))
            yield return node with { Name = $"Tough {node.Name}" };
    }
}