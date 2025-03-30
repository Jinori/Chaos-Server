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
                MaximumHp = 500
            }
        },
        {
            3, new Attributes
            {
                MaximumMp = 500
            }
        },
        {
            4, new Attributes
            {
                SpellDamagePct = 8
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