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
                MaximumHp = 1500,
                MaximumMp = 750,
                Dmg = 4,
                AtkSpeedPct = 4,
                Hit = 2,
            }
        },
        {
            3, new Attributes
            {
                SkillDamagePct = 5,
                SpellDamagePct = 5,
                FlatSkillDamage = 50,
                FlatSpellDamage = 50,
            }
        },
        {
            4, new Attributes
            {
                Ac = -3,
                MagicResistance = 3,
                CooldownReductionPct = 3
            }
        }
    };

    protected override HashSet<string> SetItemTemplateKeys { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "nyxembrace",
        "nyxwhisper",
        "nyxtwilightband",
        "nyxumbralshield",
        "grandnyxumbralshield",
        "goodnyxumbralshield",
        "greatnyxumbralshield",
    };

    public NyxSetBonusScript(Item subject)
        : base(subject) { }
}