using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class HastyPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public HastyPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<HastyPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template) => IPrefixEnchantmentScript.Mutate<HastyPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        AtkSpeedPct = 2,
        FlatSkillDamage = 10
    };

    /// <inheritdoc />
    public static string PrefixStr { get; } = "Hasty";
}