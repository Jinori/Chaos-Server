using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class RuthlessPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        SkillDamagePct = 2,
        FlatSkillDamage = 30,
        Ac = 1
    };

    /// <inheritdoc />
    public static string PrefixStr => "Ruthless";

    /// <inheritdoc />
    public RuthlessPrefixScript(Item subject)
        : base(subject)
        => IPrefixEnchantmentScript.ApplyPrefix<RuthlessPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<RuthlessPrefixScript>(node, template);
}