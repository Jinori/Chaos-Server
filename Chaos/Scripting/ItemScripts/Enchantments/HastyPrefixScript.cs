using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class HastyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public HastyPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Hasty";

        var attributes = new Attributes
        {
            AtkSpeedPct = 3,
            MaximumHp = -40
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Hasty"))
            yield return node with { Name = $"Hasty {node.Name}" };
    }
}