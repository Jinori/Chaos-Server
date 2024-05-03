using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class EternalPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public EternalPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Eternal";

        var attributes = new Attributes
        {
            Ac = -1,
            MaximumHp = 200
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Eternal"))
            yield return node with { Name = $"Eternal {node.Name}" };
    }
}