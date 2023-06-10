using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class MeagerPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MeagerPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Meager"))
            subject.DisplayName = $"Meager {subject.DisplayName}";

        var attributes = new Attributes
        {
           MaximumMp = 25,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Meager"))
            yield return node with { Name = $"Meager {node.Name}" };
    }
}