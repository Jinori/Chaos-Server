using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class ModestPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ModestPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Modest"))
            subject.DisplayName = $"Modest {subject.DisplayName}";

        var attributes = new Attributes
        {
           Ac = -2,
           MaximumHp = -250,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Modest"))
            yield return node with { Name = $"Modest {node.Name}" };
    }
}