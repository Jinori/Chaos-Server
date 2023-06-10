using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class RuthlessPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public RuthlessPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Ruthless"))
            subject.DisplayName = $"Ruthless {subject.DisplayName}";

        var attributes = new Attributes
        {
           AtkSpeedPct = 10,
            Dmg = 6,
            Str = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Ruthless"))
            yield return node with { Name = $"Ruthless {node.Name}" };
    }
}