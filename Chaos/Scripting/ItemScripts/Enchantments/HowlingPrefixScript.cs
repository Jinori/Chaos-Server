using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class HowlingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HowlingPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Howling";

        var attributes = new Attributes
        {
            Ac = 1,
            SpellDamagePct = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Howling"))
            yield return node with
            {
                Name = $"Howling {node.Name}"
            };
    }
}