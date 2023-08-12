using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class CripplingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public CripplingPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Crippling"))
            subject.DisplayName = $"Crippling {subject.DisplayName}";

        var attributes = new Attributes
        {
            MaximumHp = -350,
            FlatSkillDamage = 15
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Crippling"))
            yield return node with { Name = $"Crippling {node.Name}" };
    }
}