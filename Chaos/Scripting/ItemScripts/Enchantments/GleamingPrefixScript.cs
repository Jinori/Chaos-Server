using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class GleamingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public GleamingPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Gleaming";

        var attributes = new Attributes
        {
            SpellDamagePct = 6,
            Hit = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Gleaming"))
            yield return node with
            {
                Name = $"Gleaming {node.Name}"
            };
    }
}