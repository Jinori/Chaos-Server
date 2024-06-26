using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SavagePrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public SavagePrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Savage";

        var attributes = new Attributes
        {
            AtkSpeedPct = 1,
            Dmg = 3
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Savage"))
            yield return node with { Name = $"Savage {node.Name}" };
    }
}