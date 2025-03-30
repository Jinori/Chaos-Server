using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class SoothingPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        SpellDamagePct = 1,
        MaximumMp = 125
    };

    /// <inheritdoc />
    public static string PrefixStr => "Soothing";

    /// <inheritdoc />
    public SoothingPrefixScript(Item subject)
        : base(subject)
        => IPrefixEnchantmentScript.ApplyPrefix<SoothingPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<SoothingPrefixScript>(node, template);
}