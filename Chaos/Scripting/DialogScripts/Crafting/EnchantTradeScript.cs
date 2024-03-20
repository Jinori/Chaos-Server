using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class EnchantTradeScript : DialogScriptBase
{
    private readonly IDialogFactory DialogFactory;

    private readonly List<string> EssenceTemplateKeys = new()
    {
        "essenceofignatar", "essenceofgeolith", "essenceofzephyra", "essenceofaquaedon", "essenceofskandara", "essenceofmiraelis",
        "essenceoftheselene", "essenceofserendael"
    };
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public EnchantTradeScript(
        Dialog subject,
        IItemFactory itemFactory,
        IDialogFactory dialogFactory
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "enchant_trade_initial":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "enchant_trade_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "enchant_trade_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
            case "new_essence_pick":
            {
                OnDisplayingEssences(source);

                break;
            }
        }
    }

    public void OnDisplayingAccepted(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(1, out var previous))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var essence = ItemFactory.Create(previous.ToLowerInvariant().Replace(" ", string.Empty));

        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item) || item.Count < 1)
        {
            Subject.Reply(source, "You ran out of those essences to trade.", "enchant_trade_initial");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 1);

        source.GiveItemOrSendToBank(essence);

        Subject.InjectTextParameters(item.DisplayName, essence.DisplayName);
    }

    public void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item) || item.Count < 1)
        {
            Subject.Reply(source, "Skip", "enchant_trade_initial");

            return;
        }

        if (!EssenceTemplateKeys.Contains(item.Template.TemplateKey.ToLower()))
            Subject.Reply(source, "Essence cannot be traded", "enchant_trade_initial");
    }

    public void OnDisplayingEssences(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var slot))
            return;

        var selectedItem = source.Inventory.FirstOrDefault(x => x.Slot == slot);

        if (selectedItem == null)
            return;

        foreach (var essenceTemplateKey in EssenceTemplateKeys)
            if (!string.Equals(selectedItem.Template.TemplateKey, essenceTemplateKey, StringComparison.CurrentCultureIgnoreCase))
            {
                var fauxEssenceItem = ItemFactory.CreateFaux(essenceTemplateKey);
                Subject.Items.Add(ItemDetails.DisplayRecipe(fauxEssenceItem));
            }
    }

    public void OnDisplayingShowPlayerItems(Aisling source) => Subject.Slots =
        source.Inventory.Where(x => EssenceTemplateKeys.Contains(x.Template.TemplateKey.ToLower()))
              .Select(x => x.Slot)
              .ToList();

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.ToLower() == "new_essence_pick")
        {
            if (!Subject.MenuArgs.TryGet<string>(1, out var previous))
            {
                Subject.ReplyToUnknownInput(source);

                return;
            }

            var itemDetails = Subject.Items.FirstOrDefault(x => x.Item.DisplayName.EqualsI(previous));
            var items = itemDetails?.Item;

            if (items == null)
            {
                Subject.ReplyToUnknownInput(source);

                return;
            }

            // Find the slot of the essence item the player wants to trade.
            if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
            {
                Subject.Reply(source, "You ran out of those essences to trade.", "enchant_trade_initial");

                return;
            }

            // Create the new essence item based on the player's selection.
            var essence = ItemFactory.CreateFaux(items.Template.TemplateKey);

            // Proceed to the confirmation dialog.
            Subject.Close(source);
            var dialog = DialogFactory.Create("enchant_trade_confirmation", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(essence.DisplayName, item.DisplayName);
            dialog.Display(source);
        }
    }
}