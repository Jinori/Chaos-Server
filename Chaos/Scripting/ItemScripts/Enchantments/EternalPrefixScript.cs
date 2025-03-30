using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class EternalPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        MaximumHp = 200,
        SkillDamagePct = 1
    };

    /// <inheritdoc />
    public static string PrefixStr => "Eternal";

    /// <inheritdoc />
    public EternalPrefixScript(Item subject)
        : base(subject)
        => IPrefixEnchantmentScript.ApplyPrefix<EternalPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<EternalPrefixScript>(node, template);
}