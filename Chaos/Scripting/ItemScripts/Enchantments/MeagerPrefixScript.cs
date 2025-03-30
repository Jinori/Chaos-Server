using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class MeagerPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MaximumMp = 25
    };

    /// <inheritdoc />
    public static string PrefixStr => "Meager";

    /// <inheritdoc />
    public MeagerPrefixScript(Item subject)
        : base(subject)
        => IPrefixEnchantmentScript.ApplyPrefix<MeagerPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<MeagerPrefixScript>(node, template);
}