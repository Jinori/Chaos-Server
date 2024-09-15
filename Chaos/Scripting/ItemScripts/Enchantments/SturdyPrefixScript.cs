using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SturdyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SturdyPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Sturdy";

        var attributes = new Attributes
        {
            Ac = -1,
            MaximumHp = 400
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Sturdy"))
            yield return node with { Name = $"Sturdy {node.Name}" };
    }
}