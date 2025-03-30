using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class MightyPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        Dmg = 2
    };

    /// <inheritdoc />
    public static string PrefixStr => "Mighty";

    /// <inheritdoc />
    public MightyPrefixScript(Item subject)
        : base(subject)
        => IPrefixEnchantmentScript.ApplyPrefix<MightyPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<MightyPrefixScript>(node, template);
}