using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ModestPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ModestPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Modest";

        var attributes = new Attributes
        {
            Ac = -2,
            MaximumHp = -250
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Modest"))
            yield return node with { Name = $"Modest {node.Name}" };
    }
}