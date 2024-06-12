using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SerenePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SerenePrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Serene";

        var attributes = new Attributes
        {
            MaximumMp = 40,
            MaximumHp = 80
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Serene"))
            yield return node with { Name = $"Serene {node.Name}" };
    }
}