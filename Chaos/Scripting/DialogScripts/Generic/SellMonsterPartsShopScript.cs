using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class SellMonsterPartsShopScript : DialogScriptBase
{
    private readonly ISellShopSource SellShopSource;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SellMonsterPartsShopScript> Logger;

    // Monster Extract dictionary based on item level ranges
    private readonly Dictionary<string, (int minLevel, int maxLevel)> MonsterExtractMapping = new()
    {
        { "lessermonsterextract", (1, 40) },
        { "basicmonsterextract", (41, 70) },
        { "greatermonsterextract", (71, 96) },
        { "superiormonsterextract", (97, 120) }, 
        { "primemonsterextract", (120, int.MaxValue) }
    };

    // Dictionary for display names of Monster Extracts
    private readonly Dictionary<string, string> MonsterExtractDisplayNameMapping = new()
    {
        { "lessermonsterextract", "Lesser Monster Extract" },
        { "basicmonsterextract", "Basic Monster Extract" },
        { "greatermonsterextract", "Greater Monster Extract" },
        { "superiormonsterextract", "Superior Monster Extract" },
        { "primemonsterextract", "Prime Monster Extract" }
    };

    // Dictionary to store custom sell values for each monster part
    private readonly Dictionary<string, int> MonsterPartSellValueDictionary = new()
    {
        { "centipedegland", 1 }, // Add other monster parts with their sell values here
    };

    public SellMonsterPartsShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ILogger<SellMonsterPartsShopScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SellShopSource = (ISellShopSource)subject.DialogSource ?? throw new ArgumentNullException(nameof(subject.DialogSource));
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_sellmonsterpartsshop_initial":
                OnDisplayingInitial(source);
                break;
            case "generic_sellmonsterpartsshop_amountrequest":
                OnDisplayingAmountRequest(source);
                break;
            case "generic_sellmonsterpartsshop_confirmation":
                OnDisplayingConfirmation(source);
                break;
            case "generic_sellmonsterpartsshop_accepted":
                OnDisplayingAccepted(source);
                break;
            default:
                Subject.Reply(source, "Invalid dialog state.", "generic_error");
                break;
        }
    }

    protected virtual void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || item?.Template == null
            || !MonsterPartSellValueDictionary.ContainsKey(item.Template.TemplateKey))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        if (total < amount)
        {
            Subject.Reply(source, $"You don't have enough {item.DisplayName}(s) to sell",
                "generic_sellmonsterpartsshop_initial");
            return;
        }

        // Get the custom sell value from the dictionary
        var sellValue = MonsterPartSellValueDictionary.ContainsKey(item.Template.TemplateKey)
            ? MonsterPartSellValueDictionary[item.Template.TemplateKey] * amount
            : item.Template.SellValue * amount; // Fallback to template value if not found

        // Get the appropriate Monster Extract type based on the item's level
        var extractTypeKey = GetMonsterExtractType(item.Template.Level);
        var extractTypeName = MonsterExtractDisplayNameMapping.ContainsKey(extractTypeKey)
            ? MonsterExtractDisplayNameMapping[extractTypeKey]
            : extractTypeKey; // Fallback to the key if name is not found

        if (string.IsNullOrEmpty(extractTypeKey))
        {
            Subject.Reply(source, "No appropriate Monster Extract found for this item.",
                "generic_sellmonsterpartsshop_initial");
            return;
        }

        // Give the appropriate number of Monster Extracts, creating multiple stacks if needed
        int remainingExtracts = sellValue;
        int stackSize = 1000;

        while (remainingExtracts > 0)
        {
            int extractsToGive = Math.Min(remainingExtracts, stackSize);
            var extracts = ItemFactory.Create(extractTypeKey);
            extracts.Count = extractsToGive;

            source.GiveItemOrSendToBank(extracts);

            remainingExtracts -= extractsToGive;
        }

        Logger.LogDebug(
            "{@Player} sold {ItemCount} {@Item} to {@Merchant} for {TokenAmount} Monster Extracts",
            source,
            amount,
            item,
            Subject.DialogSource,
            sellValue);

        // Remove the specified quantity of items from the inventory
        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, amount);

        Subject.Reply(source, $"You sold {amount} {item.DisplayName}(s) for {sellValue} {extractTypeName}(s)",
            "generic_sellmonsterpartsshop_initial");
    }

    private string GetMonsterExtractType(int itemLevel)
    {
        foreach (var entry in MonsterExtractMapping)
        {
            if (itemLevel >= entry.Value.minLevel && itemLevel <= entry.Value.maxLevel)
            {
                return entry.Key;
            }
        }

        return string.Empty;
    }

    protected virtual void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) 
            || !source.Inventory.TryGetObject(slot, out var item) 
            || item?.Template == null
            || !MonsterPartSellValueDictionary.ContainsKey(item.Template.TemplateKey))
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
            || item?.Template == null
            || !MonsterPartSellValueDictionary.ContainsKey(item.Template.TemplateKey))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        if (total < amount)
        {
            Subject.Reply(source, $"You don't have enough {item.DisplayName}s to sell",
                "generic_sellmonsterpartsshop_initial");
            return;
        }

        // Get the extract type and custom sell value
        var extractTypeKey = GetMonsterExtractType(item.Template.Level);
        var extractTypeName = MonsterExtractDisplayNameMapping.ContainsKey(extractTypeKey)
            ? MonsterExtractDisplayNameMapping[extractTypeKey]
            : extractTypeKey; // Fallback to the key if name is not found

        var sellValue = MonsterPartSellValueDictionary.ContainsKey(item.Template.TemplateKey)
            ? MonsterPartSellValueDictionary[item.Template.TemplateKey] * amount
            : item.Template.SellValue * amount;

        Subject.InjectTextParameters(amount, item.DisplayName, sellValue, extractTypeName);
    }

    protected virtual void OnDisplayingInitial(Aisling source)
    {
        Subject.Slots = source.Inventory
            .Where(item => item?.Template != null 
                           && MonsterPartSellValueDictionary.ContainsKey(item.Template.TemplateKey))
            .Select(item => item.Slot)
            .ToList();
    }
}
