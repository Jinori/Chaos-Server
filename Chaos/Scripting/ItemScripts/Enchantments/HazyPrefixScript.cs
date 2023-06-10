using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class HazyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HazyPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Hazy"))
            subject.DisplayName = $"Hazy {subject.DisplayName}";

        var attributes = new Attributes
        {
           SpellDamagePct = 7,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Hazy"))
            yield return node with { Name = $"Hazy {node.Name}" };
    }
}