using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PotentPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PotentPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Potent";

        var attributes = new Attributes
        {
            SpellDamagePct = 2,
            FlatSpellDamage = 20
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Potent"))
            yield return node with { Name = $"Potent {node.Name}" };
    }
}