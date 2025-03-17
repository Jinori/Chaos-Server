using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class PrimalPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public PrimalPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<PrimalPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template) => IPrefixEnchantmentScript.Mutate<PrimalPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MaximumHp = 400,
        MaximumMp = 200,
        Ac = -1,
        SkillDamagePct = -1,
        FlatSkillDamage = -25,
        SpellDamagePct = -1,
        FlatSpellDamage = -25
    };

    /// <inheritdoc />
    public static string PrefixStr => "Primal";
}