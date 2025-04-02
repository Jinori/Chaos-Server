using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class ResilientPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        SpellDamagePct = 3,
        FlatSpellDamage = 50,
        Ac = 1
    };

    /// <inheritdoc />
    public static string PrefixStr => "Resilient";

    /// <inheritdoc />
    public ResilientPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<ResilientPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<ResilientPrefixScript>(node, template);
}