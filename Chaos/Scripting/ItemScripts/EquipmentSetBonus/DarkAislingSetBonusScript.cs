using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class DarkAislingSetBonusScript : SetBonusItemScriptBase
{
    protected override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumHp = 250,
                MaximumMp = 250
            }
        },
        {
            3, new Attributes
            {
                MaximumMp = 250,
                MaximumHp = 250
            }
        },
        {
            4, new Attributes
            {
                SpellDamagePct = 5,
                SkillDamagePct = 5,
                FlatSpellDamage = 50,
                FlatSkillDamage = 50
            }
        }
    };

    protected override HashSet<string> SetItemTemplateKeys { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "darkaislinggreaves",
        "darkaislingearrings",
        "darkaislinggauntlet",
        "darkaislingring"
    };

    public DarkAislingSetBonusScript(Item subject)
        : base(subject) { }
}