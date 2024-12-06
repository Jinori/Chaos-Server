using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;
public class SacredSetBonusScript : SetBonusItemScriptBase
{
    protected override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumMp = 1000
            }
        },
        {
            3, new Attributes
            {
                SpellDamagePct = 15
            }
        },
        {
            4, new Attributes
            {
                CooldownReductionPct = 10
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