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

public class BuyBugPointsShopScript : DialogScriptBase
{
    private readonly IBuyShopSource BuyShopSource;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyBugPointsShopScript> Logger;

    // Dictionary to hold custom prices for items
    private readonly Dictionary<string, (int price, int quantity)> ItemData = new()
    {
        { "blossomofbetrayal", (2, 1) },
        { "bocanbough", (1, 1) },
        { "cactusflower", (1, 1) },
        { "dochasbloom", (1, 1) },
        { "kabineblossom", (1, 1) },
        { "koboldtail", (1, 1) },
        { "lilypad", (1, 1) },
        { "passionflower", (1, 1) },
        { "raineach", (1, 1) },
        { "sparkflower", (1, 1) },
        { "bread", (1, 1) },
        { "cheese", (2, 1) },
        { "flour", (2, 1) },
        { "marinade", (3, 1) },
        { "salt", (3, 1) },
        { "Sugar", (3, 1) },
        { "greengrapes", (1, 5) },
        { "strawberry", (1, 5) },
        { "tangerines", (1, 5) },
        { "beefslices", (1, 3) },
        { "clam", (1, 3) },
        { "lobstertail", (1, 1) },
        { "rawmeat", (1, 5) },
        { "EssenceofAquaedon", (1, 2) },
        { "EssenceofGeolith", (1, 2) },
        { "EssenceofIgnatar", (1, 2) },
        { "EssenceofMiraelis", (1, 2) },
        { "EssenceofSerendael", (1, 2) },
        { "EssenceofSkandara", (1, 2) },
        { "EssenceofTheselene", (1, 2) },
        { "EssenceofZephyra", (1, 2) },
        { "cotton", (1, 5) },
        { "linen", (2, 3) },
        { "silk", (5, 1) },
        { "wool", (4, 2) },
        { "rawberyl", (1, 3) },
        { "rawemerald", (1, 3) },
        { "rawruby", (1, 3) },
        { "rawheartstone", (1, 3) },
        { "rawsapphire", (1, 3) },
        { "rawbronze", (1, 5) },
        { "rawiron", (2, 5) },
        { "rawmythril", (3, 5) },
        { "rawhybrasyl", (4, 2) },
        { "rawcrimsonite", (5, 2) },
        { "rawazurium", (5, 2) },
        { "sweetbuns", (2, 1) },
        { "passport", (1, 5) },
        { "lessermonsterextract", (1, 5) },
        { "basicmonsterextract", (2, 4) },
        { "greatermonsterextract", (3, 3) },
        { "superiormonsterextract", (4, 2) },
        { "primemonsterextract", (5, 1) }

    };

    /// <inheritdoc />
    public BuyBugPointsShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        ILogger<BuyBugPointsShopScript> logger
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
            case "generic_buyshopbp_initial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_buyshopbp_amountrequest":
            {
                OnDisplayingAmountRequest(source);
                break;
            }
            case "generic_buyshopbp_confirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_buyshopbp_accepted":
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

    // Fetch custom price and quantity from the dictionary
    if (!ItemData.TryGetValue(item.Template.TemplateKey, out var itemInfo))
    {
        Subject.Reply(source, "Item not found.", "generic_buyshopbp_initial");
        return;
    }

    var (itemPrice, itemQuantityPerPurchase) = itemInfo;

    var totalCost = itemPrice * amount;

    // Check if the player has enough bug report points
    if (!source.Trackers.Counters.TryGetValue("bugreportpoints", out var currentPoints) || currentPoints < totalCost)
    {
        Subject.Reply(source, $"You don't have enough Bug Report Points. You need {totalCost} Bug points.", "generic_buyshopbp_initial");
        return;
    }

    // Calculate the total quantity to give based on the quantity per purchase
    var totalQuantityToGive = itemQuantityPerPurchase * amount;

    // Perform item transaction
    var buyItemResult = ComplexActionHelper.BuyBugReportPointItem(
        source,
        BuyShopSource,
        item,
        ItemFactory,
        ItemCloner,
        totalQuantityToGive, // Use the calculated quantity
        itemPrice);

    switch (buyItemResult)
    {
        case ComplexActionHelper.BuyBugPointItemResult.Success:
            Logger.LogDebug(
                "{@Player} bought {ItemCount} {@Item} from {@Merchant} for {BugReportPoints} bug report points",
                source,
                totalQuantityToGive,
                item,
                Subject.DialogSource,
                totalCost);
            Subject.Reply(source, $"You bought {totalQuantityToGive} {item.DisplayName}(s) for {totalCost} Bug Report Points.", "generic_buyshopbp_initial");
            break;
        case ComplexActionHelper.BuyBugPointItemResult.CantCarry:
            Subject.Reply(source, "You can't carry that many items.", "generic_buyshopbp_initial");
            break;
        case ComplexActionHelper.BuyBugPointItemResult.NotEnoughStock:
            var availableStock = BuyShopSource.GetStock(item.Template.TemplateKey);
            Subject.Reply(source, $"Sorry, only {availableStock} {item.DisplayName}(s) in stock.", "generic_buyshopbp_initial");
            break;
        case ComplexActionHelper.BuyBugPointItemResult.BadInput:
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

        // Fetch custom price and quantity from the dictionary
        if (!ItemData.TryGetValue(item.Template.TemplateKey, out var itemInfo))
        {
            Subject.Reply(source, "Item price not found.", "generic_buyshopbp_initial");
            return;
        }

        var (itemPrice, itemQuantity) = itemInfo;

        // Inject item name, price, and quantity into the dialog parameters
        Subject.InjectTextParameters(item.DisplayName, itemQuantity, itemPrice);
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
            Subject.Reply(source, $"Sorry, only {availableStock} {item.DisplayName}(s) in stock.", "generic_buyshopbp_initial");
            return;
        }

        // Fetch custom price and quantity from the dictionary
        if (!ItemData.TryGetValue(item.Template.TemplateKey, out var itemInfo))
        {
            Subject.Reply(source, "Item price not found.", "generic_buyshopbp_initial");
            return;
        }

        var (itemPrice, itemQuantity) = itemInfo;

        Subject.InjectTextParameters(amount, item.DisplayName, itemQuantity, itemQuantity * amount, itemPrice * amount);
    }


    protected virtual void OnDisplayingInitial(Aisling source)
    {
        foreach (var item in BuyShopSource.ItemsForSale)
        {
            if (BuyShopSource.HasStock(item.Template.TemplateKey))
            {
                // Get the price and quantity from the dictionary or use default values if not found
                if (ItemData.TryGetValue(item.Template.TemplateKey, out var itemInfo))
                {
                    var (price, quantity) = itemInfo;

                    // Pass the custom price and quantity to the BuyWithBp method
                    Subject.Items.Add(ItemDetails.BuyWithBp(item, price)); // Assuming BuyWithBp can take just the price
                }
                else
                {
                    // Handle case where item is not found in the dictionary
                    Subject.Items.Add(ItemDetails.BuyWithBp(item, 0)); // Default to 0 if item is not found
                }
            }
        }
    }


}
