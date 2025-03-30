using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class HemlochPotionScript : ItemScriptBase
{
    public HemlochPotionScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity("Hemloch", 1);

        //Let's restore their maximums
        source.StatSheet.SetHp(1);

        //Refresh the users health bar
        source.Client.SendAttributes(StatUpdateType.Vitality);

        //Let's tell the player they have been revived
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You suddenly feel weak.");
    }
}