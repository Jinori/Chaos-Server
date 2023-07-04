using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class FocusedPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public FocusedPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Focused"))
            subject.DisplayName = $"Focused {subject.DisplayName}";

        var attributes = new Attributes
        {
            Hit = 1,
            Dmg = -1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Focused"))
            yield return node with { Name = $"Focused {node.Name}" };
    }
}