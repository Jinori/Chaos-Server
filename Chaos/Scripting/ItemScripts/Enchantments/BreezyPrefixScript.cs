using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class BreezyPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public BreezyPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<BreezyPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<BreezyPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        SpellDamagePct = 1
    };

    /// <inheritdoc />
    public static string PrefixStr => "Breezy";
}