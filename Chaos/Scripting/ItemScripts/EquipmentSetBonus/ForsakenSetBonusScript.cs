using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;
public class ForsakenSetBonusScript : SetBonusItemScriptBase
{
    protected override Dictionary<int, Attributes> SetBonus { get; } = new()
    {
        {
            2, new Attributes
            {
                MaximumHp = 1000
            }
        },
        {
            3, new Attributes
            {
                AtkSpeedPct = 16
            }
        },
        {
            4, new Attributes
            {
                SkillDamagePct = 15
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