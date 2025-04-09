using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class ForsakenSetBonusScript : SetBonusItemScriptBase
{
    public override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumHp = 2000,
                Dmg = 6,
                AtkSpeedPct = 6
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