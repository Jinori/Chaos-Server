#region
using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Humanizer;
#endregion

namespace Chaos.Scripting.DialogScripts.ShopScripts;

public class SellShopScript : DialogScriptBase
{
    private readonly ILogger<SellShopScript> Logger;
    private readonly ISellShopSource SellShopSource;

    /// <inheritdoc />
    public SellShopScript(Dialog subject, ILogger<SellShopScript> logger)
        : base(subject)
    {
        Logger = logger;
        SellShopSource = (ISellShopSource)subject.DialogSource;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_sellshop_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_sellshop_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
            case "generic_sellshop_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_sellshop_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var totalSellValue = amount * item.Template.SellValue;

        var sellItemResult = ComplexActionHelper.SellItem(
            source,
            slot,
            amount,
            item.Template.SellValue);

        switch (sellItemResult)
        {
            case ComplexActionHelper.SellItemResult.Success:
                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Entities.Gold,
                          Topics.Actions.Sell)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(item)
                      .LogInformation(
                          "Aisling {@AislingName} sold {ItemAmount} {@ItemName} to merchant {@MerchantName} for {GoldAmount} gold",
                          source.Name,
                          amount,
                          item.DisplayName,
                          SellShopSource.Name,
                          totalSellValue);

                return;
            case ComplexActionHelper.SellItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many.", "generic_sellshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.", "generic_sellshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.ItemDamaged:
                Subject.Reply(source, "I don't want your junk, ask a smith to fix it.", "generic_sellshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item) || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        if (total == 1)
        {
            Subject.MenuArgs.Add("1");
            Subject.Next(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName, total);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var itemName = GetDisplayNameWithPlural(item.DisplayName, amount);
        
        Subject.InjectTextParameters(amount, itemName, amount * item.Template.SellValue);
    }
    
    private static string GetDisplayNameWithPlural(string baseName, int amount)
    {
        if ((amount == 1) || string.IsNullOrWhiteSpace(baseName))
            return baseName;

        return baseName.Pluralize();
    }
    
    private void OnDisplayingInitial(Aisling source)
        => Subject.Slots = source.Inventory
                                 .Where(SellShopSource.IsBuying)
                                 .Select(item => item.Slot)
                                 .ToList();
}