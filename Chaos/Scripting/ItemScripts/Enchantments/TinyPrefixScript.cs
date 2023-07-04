using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class TinyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public TinyPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Tiny"))
            subject.DisplayName = $"Tiny {subject.DisplayName}";

        var attributes = new Attributes
        {
            MaximumMp = 75
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Tiny"))
            yield return node with { Name = $"Tiny {node.Name}" };
    }
}