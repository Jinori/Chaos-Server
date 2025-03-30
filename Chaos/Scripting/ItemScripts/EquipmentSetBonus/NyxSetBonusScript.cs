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
                AtkSpeedPct = 12,
                CooldownReductionPct = 3
            }
        },
        {
            4, new Attributes
            {
                SkillDamagePct = 15,
                SpellDamagePct = 10,
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