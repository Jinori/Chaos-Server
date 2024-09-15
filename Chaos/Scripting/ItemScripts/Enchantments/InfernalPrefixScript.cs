using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class InfernalPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public InfernalPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Infernal";

        var attributes = new Attributes
        {
            Ac = 1,
            AtkSpeedPct = 5,
            FlatSkillDamage = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Infernal"))
            yield return node with { Name = $"Infernal {node.Name}" };
    }
}