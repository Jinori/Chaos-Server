using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class CursedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public CursedPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Cursed";

        var attributes = new Attributes
        {
            MagicResistance = -5,
            AtkSpeedPct = 3,
            Dmg = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Cursed"))
            yield return node with { Name = $"Cursed {node.Name}" };
    }
}