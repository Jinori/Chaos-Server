using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class WhirlingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public WhirlingPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Whirling"))
            subject.DisplayName = $"Whirling {subject.DisplayName}";

        var attributes = new Attributes
        {
           SpellDamagePct = 10,
            FlatSpellDamage = 15,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Whirling"))
            yield return node with { Name = $"Whirling {node.Name}" };
    }
}