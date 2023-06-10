using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class MightyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MightyPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Mighty"))
            subject.DisplayName = $"Mighty {subject.DisplayName}";

        var attributes = new Attributes
        {
           FlatSkillDamage = 10
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Mighty"))
            yield return node with { Name = $"Mighty {node.Name}" };
    }
}