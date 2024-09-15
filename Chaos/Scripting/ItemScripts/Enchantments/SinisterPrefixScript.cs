using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SinisterPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SinisterPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Sinister";

        var attributes = new Attributes
        {
            FlatSkillDamage = 5,
            MaximumHp = -200,
            Dmg = 4
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Sinister"))
            yield return node with { Name = $"Sinister {node.Name}" };
    }
}