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

public class BuyPlatinumTokenShopScript : DialogScriptBase
{
    private readonly IBuyShopSource BuyShopSource;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuySilverTokenShopScript> Logger;

    /// <inheritdoc />
    public BuyPlatinumTokenShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        ILogger<BuySilverTokenShopScript> logger
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
            case "generic_buyplatinumtoken_initial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_buyplatinumtoken_amountrequest":
            {
                OnDisplayingAmountRequest(source);
                break;
            }
            case "generic_buyplatinumtoken_confirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_buyplatinumtoken_accepted":
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
        var bossTokens = !source.Inventory.HasCountByTemplateKey("platinumtoken", totalCost);
        

        if (bossTokens)
        {
            Subject.Reply(source, $"You don't have enough Platinum Tokens, you need {totalCost} tokens", "generic_buyplatinumtoken_initial");
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
                    "{@Player} bought {ItemCount} {@Item} from {@Merchant} for {TokenAmount} Platinum Tokens",
                    source,
                    amount,
                    item,
                    Subject.DialogSource,
                    totalCost);

                source.Inventory.RemoveQuantityByTemplateKey("silvertoken", totalCost);
                
                break;
            case ComplexActionHelper.BuyItemResult.CantCarry:
                Subject.Reply(source, "You can't carry that many", "generic_buyplatinumtoken_initial");
                break;
            case ComplexActionHelper.BuyItemResult.NotEnoughStock:
                var availableStock = BuyShopSource.GetStock(item.Template.TemplateKey);
                Subject.Reply(
                    source,
                    $"Sorry, we only have {availableStock} {item.DisplayName}s in stock",
                    "generic_buyplatinumtoken_initial");
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
                "generic_buyplatinumtoken_initial");
            return;
        }

        Subject.InjectTextParameters(amount, item.DisplayName, item.Template.BuyCost * amount);
    }

    protected virtual void OnDisplayingInitial(Aisling source)
    {
        foreach (var item in BuyShopSource.ItemsForSale)
        {
            if (BuyShopSource.HasStock(item.Template.TemplateKey))
                Subject.Items.Add(ItemDetails.BuyWithTokens(item));
        }
    }
}