using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class RevivePotionScript : ConfigurableItemScriptBase
{
    protected EquipmentType EquipmentType { get; init; }

    public RevivePotionScript(Item subject)
        : base(subject) { }

    public override bool CanUse(Aisling source)
    {
        if (source.IsAlive)
        {
            source.SendOrangeBarMessage("You must be dead to use this potion.");

            return false;
        }

        if (!source.IsAlive)
        {
            if (source.Trackers.TimedEvents.HasActiveEvent("revivepotion", out _))
            {
                source.SendOrangeBarMessage("You have already used a revive potion too recently.");

                return false;
            }

            return true;
        }

        return false;
    }

    public override void OnUse(Aisling source)
    {
        source.IsDead = false;
        source.Inventory.RemoveQuantity("Revive Potion", 1);
        //Let's restore their maximums
        source.StatSheet.AddHp(source.StatSheet.MaximumHp);
        source.StatSheet.AddMp(source.StatSheet.MaximumMp);

        //Refresh the users health bar
        source.Client.SendAttributes(StatUpdateType.Vitality);

        //Let's tell the player they have been revived
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived.");
        source.Refresh(true);
        source.Trackers.TimedEvents.AddEvent("revivepotion", TimeSpan.FromMinutes(15), true);
    }
}