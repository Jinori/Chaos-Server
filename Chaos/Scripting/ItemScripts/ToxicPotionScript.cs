using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class ToxicPotionScript : ConfigurableItemScriptBase
{
    private int? DmgAmount { get; init; }
    private int? DmgPercent { get; init; }

    public ToxicPotionScript(Item subject)
        : base(subject)
    {
    }

    public override void OnUse(Aisling source)
    {
        if (!source.IsAlive) 
            return;
        
        var amount = DmgAmount ?? source.StatSheet.MaximumHp / 100 * DmgPercent;

        //Let's remove Hp
        source.StatSheet.SubtractHp(amount!.Value);

        //Refresh the users health bar
        source.Client.SendAttributes(StatUpdateType.Vitality);

        //Update inventory quantity
        source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);
    }
}