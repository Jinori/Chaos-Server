using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class EternalPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public EternalPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Eternal"))
            subject.DisplayName = $"Eternal {subject.DisplayName}";

        var attributes = new Attributes
        {
            Ac = -1,
            Con = 1,
            MaximumHp = 250
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Eternal"))
            yield return node with { Name = $"Eternal {node.Name}" };
    }
}