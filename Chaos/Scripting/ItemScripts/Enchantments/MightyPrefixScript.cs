using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class MightyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MightyPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Mighty";

        var attributes = new Attributes
        {
            SkillDamagePct = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Mighty"))
            yield return node with { Name = $"Mighty {node.Name}" };
    }
}