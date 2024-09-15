using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PrimalPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PrimalPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Primal";

        var attributes = new Attributes
        {
            Ac = -1,
            MaximumHp = 500,
            MaximumMp = 250
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Primal"))
            yield return node with { Name = $"Primal {node.Name}" };
    }
}