using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class PotentPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PotentPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Potent"))
            subject.DisplayName = $"Potent {subject.DisplayName}";

        var attributes = new Attributes
        {
            MaximumMp = 150
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Potent"))
            yield return node with { Name = $"Potent {node.Name}" };
    }
}