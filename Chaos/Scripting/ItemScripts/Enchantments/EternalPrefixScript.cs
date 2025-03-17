using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class EternalPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public EternalPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<EternalPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<EternalPrefixScript>(node, template);
    
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        Ac = -1,
        MaximumHp = 200
    };

    /// <inheritdoc />
    public static string PrefixStr => "Eternal";
}
