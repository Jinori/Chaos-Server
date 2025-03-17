using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public sealed class BlazingPrefixScript : ItemScriptBase, IPrefixEnchantmentScript
{
    /// <inheritdoc />
    public BlazingPrefixScript(Item subject)
        : base(subject) => IPrefixEnchantmentScript.ApplyPrefix<BlazingPrefixScript>(subject);

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
        => IPrefixEnchantmentScript.Mutate<BlazingPrefixScript>(node, template);

    /// <inheritdoc />
    public static Attributes Modifiers { get; } = new()
    {
        Ac = 1,
        SkillDamagePct = 5
    };

    /// <inheritdoc />
    public static string PrefixStr => "Blazing";
}