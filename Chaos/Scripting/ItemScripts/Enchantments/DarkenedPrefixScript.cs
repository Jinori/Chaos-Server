using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class DarkenedPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public DarkenedPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<DarkenedPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<DarkenedPrefixScript>(node, template);
    
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        SpellDamagePct = 2,
        Ac = 1
    };

    /// <inheritdoc />
    public static string PrefixStr => "Darkened";
}