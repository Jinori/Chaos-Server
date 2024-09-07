using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class SellMythicTokenShopScript : DialogScriptBase
{
    private readonly ISellShopSource SellShopSource;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SellMythicTokenShopScript> Logger;

    /// <inheritdoc />
    public SellMythicTokenShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ICloningService<Item> itemCloner,
        ILogger<SellMythicTokenShopScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        ItemCloner = itemCloner;
        Logger = logger;
        SellShopSource = (ISellShopSource)Subject.DialogSource;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_sellmythictokenshop_initial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_sellmythictokenshop_amountrequest":
            {
                OnDisplayingAmountRequest(source);
                break;
            }
            case "generic_sellmythictokenshop_confirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_sellmythictokenshop_accepted":
            {
                OnDisplayingAccepted(source);
                break;
            }
        }
    }

    protected virtual void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);
        var totalSalePrice = item.Template.SellValue * amount;

        if (total < amount)
        {
            Subject.Reply(source, $"You don't have enough {item.DisplayName}'s to sell", "generic_sellmythictokenshop_initial");
            return;
        }

        // Give the appropriate number of mythictokens, creating multiple stacks if needed
        int remainingTokens = totalSalePrice;
        int stackSize = 1000; // Example stack size, adjust according to your game's mechanics

        while (remainingTokens > 0)
        {
            int tokensToGive = Math.Min(remainingTokens, stackSize);
            var tokens = ItemFactory.Create("mythictoken");
            tokens.Count = tokensToGive;

            source.GiveItemOrSendToBank(tokens);

            remainingTokens -= tokensToGive;
        }

        Logger.LogDebug(
            "{@Player} sold {ItemCount} {@Item} to {@Merchant} for {TokenAmount} Mythic Tokens",
            source,
            amount,
            item,
            Subject.DialogSource,
            totalSalePrice);

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, amount);

        Subject.Reply(source, $"You sold {amount} {item.DisplayName}(s) for {totalSalePrice} Mythic Tokens",
            "generic_sellmythictokenshop_initial");
    }

    protected virtual void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item) ||
            !SellShopSource.IsBuying(item))
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

    protected virtual void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);
            return; 
        }
        
        var total = source.Inventory.CountOf(item.DisplayName);

        if (total < amount)
        {
            Subject.Reply(
                source,
                $"You don't have enough {item.DisplayName}s to sell",
                "generic_sellmythictokenshop_initial");
            return;
        }

        Subject.InjectTextParameters(amount, item.DisplayName, item.Template.SellValue * amount);
    }

    protected virtual void OnDisplayingInitial(Aisling source)
        => Subject.Slots = source.Inventory
            .Where(SellShopSource.IsBuying)
            .Select(item => item.Slot)
            .ToList();
}
