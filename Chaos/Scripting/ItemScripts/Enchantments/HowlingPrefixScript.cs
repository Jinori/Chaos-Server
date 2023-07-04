using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class HowlingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HowlingPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Howling"))
            subject.DisplayName = $"Howling {subject.DisplayName}";

        var attributes = new Attributes
        {
            Ac = 2,
            Int = 1,
            SpellDamagePct = 12
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Howling"))
            yield return node with { Name = $"Howling {node.Name}" };
    }
}