using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class DarkAislingSetBonusScript : SetBonusItemScriptBase
{
    public override Dictionary<int, Attributes> SetBonus { get; } = new()
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
                SpellDamagePct = 3,
                SkillDamagePct = 3,
                FlatSpellDamage = 30,
                FlatSkillDamage = 30
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