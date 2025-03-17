using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class BrightPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public BrightPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<BrightPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<BrightPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MaximumMp = 150
    };

    /// <inheritdoc />
    public static string PrefixStr => "Bright";
}