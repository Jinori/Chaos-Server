using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class LightPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public LightPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Light"))
            subject.DisplayName = $"Light {subject.DisplayName}";

        var attributes = new Attributes
        {
           Hit = 5,
            Int = -2
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Light"))
            yield return node with { Name = $"Light {node.Name}" };
    }
}