using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ValiantPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ValiantPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Valiant";

        var attributes = new Attributes
        {
            SkillDamagePct = 2
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