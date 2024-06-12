using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class RuthlessPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public RuthlessPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Ruthless";

        var attributes = new Attributes
        {
            AtkSpeedPct = 3,
            SkillDamagePct = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Ruthless"))
            yield return node with { Name = $"Ruthless {node.Name}" };
    }
}