using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class MiraelisSuffixScript : ItemScriptBase, IEnchantmentScript
{
    private const string SUFFIX = " of Miraelis";
    /// <inheritdoc />
    public MiraelisSuffixScript(Item subject)
        : base(subject)
    {
        
        if (!subject.DisplayName.EndsWithI(SUFFIX))
            subject.DisplayName = $"{subject.DisplayName}{SUFFIX}";

        var attributes = new Attributes
        {
            Int = 1
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.EndsWithI(SUFFIX))
            yield return node with { Name = $"{node.Name}{SUFFIX}" };
    }
}