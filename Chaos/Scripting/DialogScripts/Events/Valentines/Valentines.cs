using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Valentines;

public class Valentines(Dialog subject, IEffectFactory effectFactory, IItemFactory itemFactory) 
    : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        if (Subject.DialogSource is not Merchant merchant) return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "aidan_polyppurplemake":
                HandleItemExchange(source, "polypsac", 3, "grape", 30, 5000000, "purpleheartpuppet");
                break;

            case "aidan_polypredmake":
                HandleItemExchange(source, "polypsac", 3, "cherry", 30, 5000000, "redheartpuppet");
                break;

            case "nadia_initial":
                HandleEventCandy(source, merchant);
                break;
        }
    }

    private void HandleItemExchange(Aisling source, string item1, int quantity1, 
                                    string item2, int quantity2, int goldRequired, string rewardItem)
    {
        if (!source.Inventory.HasCountByTemplateKey(item1, quantity1) ||
            !source.Inventory.HasCountByTemplateKey(item2, quantity2) ||
            source.Gold < goldRequired)
        {
            Subject.Reply(source, "You don't have the required items or gold.");
            return;
        }

        if (!source.TryTakeGold(goldRequired))
        {
            Subject.Reply(source, $"You don't have enough gold. I need {goldRequired:N0} gold.");
            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item1, quantity1);
        source.Inventory.RemoveQuantityByTemplateKey(item2, quantity2);
        var item = itemFactory.Create(rewardItem);
        source.GiveItemOrSendToBank(item);
    }

    private void HandleEventCandy(Aisling source, Merchant merchant)
    {
        if (!EventPeriod.IsEventActive(DateTime.UtcNow, merchant.MapInstance.InstanceId))
        {
            Subject.Reply(source, "Looks like I'm all out of candies..");
            return;
        }

        if (source.Trackers.TimedEvents.HasActiveEvent("VdayBonus", out _))
        {
            Subject.Reply(source, "I already gave you candy for this event.");
            return;
        }

        var effect = effectFactory.Create("ValentinesCandy");
        source.SendOrangeBarMessage("Nadia stuffs a chocolate in your face. Knowledge rates increased!");
        source.Effects.Apply(source, effect);
        source.Trackers.TimedEvents.AddEvent("VdayBonus", TimeSpan.FromDays(30), true);
    }
}
