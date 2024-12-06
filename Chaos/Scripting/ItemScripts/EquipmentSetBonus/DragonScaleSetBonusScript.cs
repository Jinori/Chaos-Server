using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;
public class DragonScaleSetBonusScript : SetBonusItemScriptBase
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
                AtkSpeedPct = 12
            }
        }
    };

    protected override HashSet<string> SetItemTemplateKeys { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "dragonscaleclaws",
        "dragonscalesword",
        "dragonscalegauntlet",
        "dragonscalering",
        "dragonscaledagger"
    };

    public DragonScaleSetBonusScript(Item subject)
        : base(subject) { }
}