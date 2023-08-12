using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class CursedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public CursedPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Cursed"))
            subject.DisplayName = $"Cursed {subject.DisplayName}";

        var attributes = new Attributes
        {
            MagicResistance = 10,
            AtkSpeedPct = 15,
            Dex = 1
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