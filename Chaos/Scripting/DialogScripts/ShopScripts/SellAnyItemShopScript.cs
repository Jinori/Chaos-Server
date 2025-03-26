using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.ShopScripts;

public class SellAnyItemShopScript : DialogScriptBase
{
    private readonly ILogger<SellAnyItemShopScript> Logger;
    private readonly ISellShopSource SellShopSource;

    /// <inheritdoc />
    public SellAnyItemShopScript(Dialog subject, ILogger<SellAnyItemShopScript> logger)
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
            case "generic_sellanyitemshop_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_sellanyitemshop_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
            case "generic_sellanyitemshop_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_sellanyitemshop_accepted":
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
            || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var totalSellValue = amount * (item.Template.SellValue / 3);
        var perItem = item.Template.SellValue / 3;
        var sellItemResult = ComplexActionHelper.SellItem(
            source,
            slot,
            amount,
            perItem);

        switch (sellItemResult)
        {
            case ComplexActionHelper.SellItemResult.Success:
                Logger.WithTopics(
                          [Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Entities.Gold,
                          Topics.Actions.Sell])
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
                Subject.Reply(source, "You don't have that many.", "generic_sellanyitemshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.", "generic_sellanyitemshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.ItemDamaged:
                Subject.Reply(source, "I don't want your junk, ask a smith to fix it.", "generic_sellanyitemshop_initial");

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
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (item.Template.SellValue is 0 or <= 1)
        {
            Subject.Reply(source, $"I cannot buy your {item.DisplayName}. It's invaluable.");

            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        if (total == 1)
        {
            Subject.MenuArgs.Add("1");
            Subject.Next(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName, total.ToWords());
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var itemName = GetDisplayNameWithPlural(item.DisplayName, amount);
        var totalValue = amount * (item.Template.SellValue / 3);

        Subject.InjectTextParameters(amount.ToWords(), itemName, totalValue);
    }
    
    private static string GetDisplayNameWithPlural(string baseName, int amount)
    {
        if (amount == 1)
            return baseName;

        // If name already ends in 's', assume it's plural enough
        if (baseName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return baseName;

        return baseName + "s";
    }

    
    private void OnDisplayingInitial(Aisling source)
        => Subject.Slots = source.Inventory
                                 .Where(x => x.Template.SellValue >= 2)
                                 .Select(item => item.Slot)
                                 .ToList();
}