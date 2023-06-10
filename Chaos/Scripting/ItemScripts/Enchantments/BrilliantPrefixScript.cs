using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class BrilliantPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BrilliantPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Brilliant"))
            subject.DisplayName = $"Brilliant {subject.DisplayName}";

        var attributes = new Attributes
        {
           Int = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Brilliant"))
            yield return node with { Name = $"Brilliant {node.Name}" };
    }
}