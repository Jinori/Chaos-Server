using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class BlazingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BlazingPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Blazing"))
            subject.DisplayName = $"Blazing {subject.DisplayName}";

        var attributes = new Attributes
        {
           Ac = 3,
            Str = 1,
            SkillDamagePct = 10,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Blazing"))
            yield return node with { Name = $"Blazing {node.Name}" };
    }
}