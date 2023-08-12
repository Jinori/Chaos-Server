using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class ValiantPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ValiantPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Valiant"))
            subject.DisplayName = $"Valiant {subject.DisplayName}";

        var attributes = new Attributes
        {
            SkillDamagePct = 6,
            AtkSpeedPct = -5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Valiant"))
            yield return node with { Name = $"Valiant {node.Name}" };
    }
}