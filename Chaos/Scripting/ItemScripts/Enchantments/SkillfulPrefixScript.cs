using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SkillfulPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SkillfulPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Skillful";

        var attributes = new Attributes
        {
            AtkSpeedPct = 2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Skillful"))
            yield return node with { Name = $"Skillful {node.Name}" };
    }
}