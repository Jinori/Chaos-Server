using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class SavagePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SavagePrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Savage"))
            subject.DisplayName = $"Savage {subject.DisplayName}";

        var attributes = new Attributes
        {
           Dmg = 5,
            Str = 1,
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Savage"))
            yield return node with { Name = $"Savage {node.Name}" };
    }
}