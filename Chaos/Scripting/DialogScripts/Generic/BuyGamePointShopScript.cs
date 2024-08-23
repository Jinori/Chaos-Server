using Chaos.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Generic;

public class BuyGamePointShopScript : DialogScriptBase
{
    private readonly IBuyShopSource BuyShopSource;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyGamePointShopScript> Logger;

    /// <inheritdoc />
    public BuyGamePointShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        ILogger<BuyGamePointShopScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        ItemCloner = itemCloner;
        Logger = logger;
        BuyShopSource = (IBuyShopSource)Subject.DialogSource;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_buyshopgp_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_buyshopgp_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
            case "generic_buyshopgp_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_buyshopgp_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    protected virtual void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string, int>(out var itemName, out var amount)
            || (amount <= 0)
            || !BuyShopSource.TryGetItem(itemName, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var totalCost = item.Template.BuyCost * amount;

        var buyItemResult = ComplexActionHelper.BuyGamePointItem(
            source,
            BuyShopSource,
            item,
            ItemFactory,
            ItemCloner,
            amount,
            item.Template.BuyCost);

        switch (buyItemResult)
        {
            case ComplexActionHelper.BuyGamePointItemResult.Success:
                Logger.LogDebug(
                    "{@Player} bought {ItemCount} {@Item} from {@Merchant} for {GpAmount} gamepoints",
                    source,
                    amount,
                    item,
                    Subject.DialogSource,
                    totalCost);

                break;
            case ComplexActionHelper.BuyGamePointItemResult.CantCarry:
                Subject.Reply(source, "You can't carry that many", "generic_buyshopgp_initial");

                break;
            case ComplexActionHelper.BuyGamePointItemResult.NotEnoughGamePoints:
                Subject.Reply(source, $"You don't have enough gamepoints, you need {totalCost} gamepoints", "generic_buyshopgp_initial");

                break;
            case ComplexActionHelper.BuyGamePointItemResult.NotEnoughStock:
                var availableStock = BuyShopSource.GetStock(item.Template.TemplateKey);

                Subject.Reply(
                    source,
                    $"Sorry, we only have {availableStock} {item.DisplayName}s in stock",
                    "generic_buyshopgp_initial");

                break;
            case ComplexActionHelper.BuyGamePointItemResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<string>(out var itemName) || !BuyShopSource.TryGetItem(itemName, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName, item.Template.BuyCost);
    }

    protected virtual void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<string, int>(out var itemName, out var amount)
            || (amount <= 0)
            || !BuyShopSource.TryGetItem(itemName, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var availableStock = BuyShopSource.GetStock(item.Template.TemplateKey);

        if (availableStock < amount)
        {
            Subject.Reply(
                source,
                $"Sorry, we only have {availableStock} {item.DisplayName}s in stock",
                "generic_buyshopgp_initial");

            return;
        }

        Subject.InjectTextParameters(amount, item.DisplayName, item.Template.BuyCost * amount);
    }

    protected virtual void OnDisplayingInitial(Aisling source)
    {
        foreach (var item in BuyShopSource.ItemsForSale)
        {
            if (item.Template.TemplateKey == "ironglove")
                if (!source.Trackers.Flags.HasFlag(ForagingQuest.Reached3000))
                    continue;
            
            if (item.Template.TemplateKey == "mythrilglove")
                if (!source.Trackers.Flags.HasFlag(ForagingQuest.Reached15000))
                    continue;
            
            if (item.Template.TemplateKey == "hybrasylglove")
                if (!source.Trackers.Flags.HasFlag(ForagingQuest.Reached50000))
                    continue;
            
            if (BuyShopSource.HasStock(item.Template.TemplateKey))
                Subject.Items.Add(ItemDetails.BuyWithGp(item));
        }
    }
}