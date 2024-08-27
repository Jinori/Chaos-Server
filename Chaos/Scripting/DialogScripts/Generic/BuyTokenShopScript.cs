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

public class BuyTokenShopScript : DialogScriptBase
{
    private readonly IBuyShopSource BuyShopSource;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyTokenShopScript> Logger;

    /// <inheritdoc />
    public BuyTokenShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        ILogger<BuyTokenShopScript> logger
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
            case "generic_buyshopbosstoken_initial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_buyshopbosstoken_amountrequest":
            {
                OnDisplayingAmountRequest(source);
                break;
            }
            case "generic_buyshopbosstoken_confirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_buyshopbosstoken_accepted":
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
        var bossTokens = !source.Inventory.HasCountByTemplateKey("bosstoken", totalCost);
        

        if (bossTokens)
        {
            Subject.Reply(source, $"You don't have enough boss tokens, you need {totalCost} tokens", "generic_buyshopbosstoken_initial");
            return;
        }

        var buyItemResult = ComplexActionHelper.BuyItem(
            source,
            BuyShopSource,
            item,
            ItemFactory,
            ItemCloner,
            amount,
            item.Template.BuyCost);

        switch (buyItemResult)
        {
            case ComplexActionHelper.BuyItemResult.Success:
                Logger.LogDebug(
                    "{@Player} bought {ItemCount} {@Item} from {@Merchant} for {TokenAmount} boss tokens",
                    source,
                    amount,
                    item,
                    Subject.DialogSource,
                    totalCost);
                break;
            case ComplexActionHelper.BuyItemResult.CantCarry:
                Subject.Reply(source, "You can't carry that many", "generic_buyshopbosstoken_initial");
                break;
            case ComplexActionHelper.BuyItemResult.NotEnoughStock:
                var availableStock = BuyShopSource.GetStock(item.Template.TemplateKey);
                Subject.Reply(
                    source,
                    $"Sorry, we only have {availableStock} {item.DisplayName}s in stock",
                    "generic_buyshopbosstoken_initial");
                break;
            case ComplexActionHelper.BuyItemResult.BadInput:
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
                "generic_buyshopbosstoken_initial");
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
                Subject.Items.Add(ItemDetails.BuyWithGp(item)); // You might want to create a new method for buying with tokens
        }
    }
}
