using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class ForsakenSetBonusScript : SetBonusItemScriptBase
{
    protected override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumHp = 1000,
                Dmg = 4,
                AtkSpeedPct = 4
            }
        },
        {
            3, new Attributes
            {
                SkillDamagePct = 8,
                FlatSkillDamage = 75
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
        "forsakenbelt",
        "forsakennecklace",
        "forsakengauntlet",
        "forsakenring"
    };

    public ForsakenSetBonusScript(Item subject)
        : base(subject) { }
}