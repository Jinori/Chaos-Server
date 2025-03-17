using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class AncientPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public AncientPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<AncientPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<AncientPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MaximumHp = 300,
        MaximumMp = 150
    };

    /// <inheritdoc />
    public static string PrefixStr => "Ancient";
}