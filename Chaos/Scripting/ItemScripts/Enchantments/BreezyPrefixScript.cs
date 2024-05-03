using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class BreezyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BreezyPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Breezy";

        var attributes = new Attributes
        {
            SpellDamagePct = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Breezy"))
            yield return node with { Name = $"Breezy {node.Name}" };
    }
}