using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class FocusedPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public FocusedPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<FocusedPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template) => IPrefixEnchantmentScript.Mutate<FocusedPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        Hit = 1,
        AtkSpeedPct = 2
    };

    /// <inheritdoc />
    public static string PrefixStr => "Focused";
}