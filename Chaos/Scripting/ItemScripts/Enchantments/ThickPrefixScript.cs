using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ThickPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public ThickPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Thick";

        var attributes = new Attributes
        {
            MaximumHp = 800,
            MaximumMp = 400
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Thick"))
            yield return node with { Name = $"Thick {node.Name}" };
    }
}