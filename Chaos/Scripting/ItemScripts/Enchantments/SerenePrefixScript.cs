using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class SerenePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SerenePrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Serene"))
            subject.DisplayName = $"Serene {subject.DisplayName}";

        var attributes = new Attributes
        {
           MaximumMp = 100,
            MaximumHp = -75,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Serene"))
            yield return node with { Name = $"Serene {node.Name}" };
    }
}