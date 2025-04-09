using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class SacredSetBonusScript : SetBonusItemScriptBase
{
    public override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumMp = 1000,
                Hit = 3
            }
        },
        {
            3, new Attributes
            {
                SpellDamagePct = 8,
                HealBonusPct = 8,
                FlatSpellDamage = 75,
                HealBonus = 75
            }
        },
        {
            4, new Attributes
            {
                CooldownReductionPct = 5,
                MagicResistance = 10
            }
        }
    };

    protected override HashSet<string> SetItemTemplateKeys { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "sacredearrings",
        "sacrednecklace",
        "sacredgreaves",
        "sacredring"
    };

    public SacredSetBonusScript(Item subject)
        : base(subject) { }
}