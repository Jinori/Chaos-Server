using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class MeagerPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MeagerPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Meager";

        var attributes = new Attributes
        {
            MaximumMp = 40
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Meager"))
            yield return node with { Name = $"Meager {node.Name}" };
    }
}