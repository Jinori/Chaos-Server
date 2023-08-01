using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class SkillfulPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SkillfulPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Skillful"))
            subject.DisplayName = $"Skillful {subject.DisplayName}";

        var attributes = new Attributes
        {
            SkillDamagePct = 3,
            AtkSpeedPct = 15
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Skillful"))
            yield return node with { Name = $"Skillful {node.Name}" };
    }
}