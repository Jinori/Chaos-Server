using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class HazyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HazyPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Hazy";

        var attributes = new Attributes
        {
            SpellDamagePct = 3,
            Ac = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Hazy"))
            yield return node with { Name = $"Hazy {node.Name}" };
    }
}