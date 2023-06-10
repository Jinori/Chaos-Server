using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class BreezyPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public BreezyPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Breezy"))
            subject.DisplayName = $"Breezy {subject.DisplayName}";

        var attributes = new Attributes
        {
           FlatSpellDamage = 10
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Breezy"))
            yield return node with { Name = $"Breezy {node.Name}" };
    }
}