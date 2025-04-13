using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PrimalPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MaximumHp = 400,
        Dmg = 2,
        AtkSpeedPct = 2,
        SkillDamagePct = 1,
        FlatSkillDamage = 20
    };

    /// <inheritdoc />
    public static string PrefixStr => "Primal";

    /// <inheritdoc />
    public PrimalPrefixScript(Item subject)
        : base(subject)
        => IPrefixEnchantmentScript.ApplyPrefix<PrimalPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<PrimalPrefixScript>(node, template);
}