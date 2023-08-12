using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class PersistingPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public PersistingPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Persisting"))
            subject.DisplayName = $"Persisting {subject.DisplayName}";

        var attributes = new Attributes
        {
            Hit = 6,
            MaximumMp = 150
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