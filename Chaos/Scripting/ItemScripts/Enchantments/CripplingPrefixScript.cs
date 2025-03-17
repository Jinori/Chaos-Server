using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class CripplingPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public CripplingPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<CripplingPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<CripplingPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        AtkSpeedPct = -1,
        SkillDamagePct = 3
    };

    /// <inheritdoc />
    public static string PrefixStr => "Crippling";
}