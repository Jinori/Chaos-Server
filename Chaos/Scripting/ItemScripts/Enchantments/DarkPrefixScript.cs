using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class DarkPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public DarkPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Dark"))
            subject.DisplayName = $"Dark {subject.DisplayName}";

        var attributes = new Attributes
        {
            Dmg = 5,
            Dex = -3
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Dark"))
            yield return node with { Name = $"Dark {node.Name}" };
    }
}