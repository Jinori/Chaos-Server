using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events.Christmas;

public class ChristmasTreeMerchant : MerchantScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public ChristmasTreeMerchant(Merchant subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "christmastree":
            {
                if (source is not Aisling aisling)
                    return;

                if (!message.EqualsI("Merry Christmas"))
                    return;

                if (aisling.Trackers.TimedEvents.HasActiveEvent("christmastree", out _))
                {
                    aisling.SendOrangeBarMessage("You've already gotten a gift!");

                    return;
                }

                aisling.Trackers.TimedEvents.AddEvent("christmastree", TimeSpan.FromHours(3), true);
                var present = ItemFactory.Create("present");
                aisling.GiveItemOrSendToBank(present);
                aisling.SendOrangeBarMessage("You've gotten a present from the tree!");
            }

                break;
        }
    }
}