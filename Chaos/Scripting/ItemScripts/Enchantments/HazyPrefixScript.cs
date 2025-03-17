using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class HazyPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public HazyPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<HazyPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template) => IPrefixEnchantmentScript.Mutate<HazyPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MagicResistance = 3,
        Ac = 1,
        Hit = -1
    };

    /// <inheritdoc />
    public static string PrefixStr => "Hazy";
}