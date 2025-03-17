using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class CursedPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public CursedPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<CursedPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<CursedPrefixScript>(node, template);
    
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MagicResistance = -5,
        AtkSpeedPct = 3,
        Dmg = 5
    };

    /// <inheritdoc />
    public static string PrefixStr => "Cursed";
}
