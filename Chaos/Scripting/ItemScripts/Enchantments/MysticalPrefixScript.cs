using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class MysticalPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MysticalPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Mystical";

        var attributes = new Attributes
        {
            SpellDamagePct = 3,
            MaximumMp = -50
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Mystical"))
            yield return node with { Name = $"Mystical {node.Name}" };
    }
}