using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PristinePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PristinePrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Pristine";

        var attributes = new Attributes
        {
            Hit = 6,
            SpellDamagePct = 3
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Pristine"))
            yield return node with { Name = $"Pristine {node.Name}" };
    }
}