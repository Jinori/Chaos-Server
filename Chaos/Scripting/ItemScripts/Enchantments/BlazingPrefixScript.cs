using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class BlazingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BlazingPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Blazing";

        var attributes = new Attributes
        {
            Ac = 1,
            SkillDamagePct = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Blazing"))
            yield return node with { Name = $"Blazing {node.Name}" };
    }
}