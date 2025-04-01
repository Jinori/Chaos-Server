using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class NyxSetBonusScript : SetBonusItemScriptBase
{
    protected override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumHp = 3000,
                MaximumMp = 1500
            }
        },
        {
            3, new Attributes
            {
                AtkSpeedPct = 6,
                Dmg = 6,
                Hit = 3,
                CooldownReductionPct = 5
            }
        },
        {
            4, new Attributes
            {
                SkillDamagePct = 5,
                SpellDamagePct = 5,
                FlatSkillDamage = 50,
                FlatSpellDamage = 50,
                Ac = -5
            }
        }
    };

    protected override HashSet<string> SetItemTemplateKeys { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "nyxembrace",
        "nyxwhisper",
        "nyxtwilightband",
        "nyxumbralshield"
    };

    public NyxSetBonusScript(Item subject)
        : base(subject) { }
}