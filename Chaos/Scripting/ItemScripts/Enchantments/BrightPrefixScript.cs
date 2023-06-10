using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class BrightPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BrightPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Bright"))
            subject.DisplayName = $"Bright {subject.DisplayName}";

        var attributes = new Attributes
        {
            FlatSpellDamage = 15,
            MaximumMp = -350,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Bright"))
            yield return node with { Name = $"Bright {node.Name}" };
    }
}