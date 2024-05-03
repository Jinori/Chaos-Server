using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PersistingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PersistingPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Persisting";

        var attributes = new Attributes
        {
            Hit = 5
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Persisting"))
            yield return node with { Name = $"Persisting {node.Name}" };
    }
}