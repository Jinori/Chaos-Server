using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts.Generic;

public class SellShopScript : ConfigurableDialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly ILogger<SellShopScript> Logger;
    private int? Amount;
    private Item? PlayerItem;
    private byte? Slot;
    private int? TotalAvailable;
    private int? TotalSellValue;
    protected List<string> ItemTemplateKeys { get; init; } = null!;

    /// <inheritdoc />
    public SellShopScript(Dialog subject, ILogger<SellShopScript> logger)
        : base(subject)
    {
        Logger = logger;

        var requestInputText =
            DialogString.From(() => $"How many {PlayerItem!.DisplayName} would you like to sell? You have {TotalAvailable}.");

        var requestOptionText = DialogString.From(
            () => $"I can buy {PlayerItem!.ToAmountString(Amount!.Value)} for {TotalSellValue} gold. Is that okay?");

        InputCollector = new InputCollectorBuilder()
                         .RequestTextInput(requestInputText)
                         .HandleInput(HandleTextInput)
                         .RequestOptionSelection(requestOptionText, DialogString.Yes, DialogString.No)
                         .HandleInput(HandleOptionSelection)
                         .Build();
    }

    private bool HandleOptionSelection(Aisling source, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;

        var result = ComplexActionHelper.SellItem(
            source,
            Slot!.Value,
            Amount!.Value,
            PlayerItem!.Template.SellValue);

        switch (result)
        {
            case ComplexActionHelper.SellItemResult.Success:
                Logger.LogDebug(
                    "{Player} sold {Item} to {Merchant} for {Amount} gold",
                    source,
                    PlayerItem!.ToAmountString(Amount!.Value),
                    Subject.SourceEntity,
                    Amount!.Value);

                return true;
            case ComplexActionHelper.SellItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many.");

                return false;
            case ComplexActionHelper.SellItemResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.");

                return false;
            case ComplexActionHelper.SellItemResult.BadInput:
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool HandleTextInput(Aisling source, Dialog dialog, int? option = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(1, out var amount) || amount <= 0)
        {
            dialog.Reply(source, DialogString.UnknownInput.Value);

            return false;
        }

        if (amount > TotalAvailable)
        {
            dialog.Reply(source, "You don't have that many to sell.");

            return false;
        }

        Amount = amount;
        TotalSellValue = PlayerItem!.Template.SellValue * amount;

        return true;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Slots.IsNullOrEmpty())
            Subject.Slots = source.Inventory.Where(i => ItemTemplateKeys.ContainsI(i.Template.TemplateKey))
                                  .Select(i => i.Slot)
                                  .ToList();
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (PlayerItem == null)
        {
            if (!Subject.MenuArgs.Any())
                return;

            if (!byte.TryParse(Subject.MenuArgs.First(), out var invSlot))
                return;

            Slot = invSlot;
            PlayerItem = source.Inventory[invSlot];

            if (PlayerItem == null)
                return;

            TotalAvailable = source.Inventory.CountOf(PlayerItem.DisplayName);
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}