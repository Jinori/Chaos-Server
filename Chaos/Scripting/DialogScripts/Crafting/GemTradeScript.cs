using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class GemTradeScript : DialogScriptBase
{
    private readonly IDialogFactory DialogFactory;

    private readonly List<string> GemTemplateKeys = new()
    {
        "rawemerald",
        "rawheartstone",
        "rawsapphire",
        "rawruby",
        "rawberyl"
    };

    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GemTradeScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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
            case "gem_trade_initial":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "gem_trade_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "gem_trade_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
            case "new_gem_pick":
            {
                OnDisplayingGems(source);

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

        var gem = ItemFactory.Create(
            previous.ToLowerInvariant()
                    .Replace(" ", string.Empty));

        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item) || (item.Count < 1))
        {
            Subject.Reply(source, "You ran out of those gems to trade.", "gem_trade_initial");

            return;
        }

        if (!source.TryTakeGold(2500))
        {
            Subject.Reply(source, "You can't afford to trade gems. I need more gold for this trade.", "plant_trade_initial");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 1);

        source.GiveItemOrSendToBank(gem);

        Subject.InjectTextParameters(item.DisplayName, gem.DisplayName);
    }

    public void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item) || (item.Count < 1))
        {
            Subject.Reply(source, "Skip", "gem_trade_initial");

            return;
        }

        if (!GemTemplateKeys.Contains(item.Template.TemplateKey.ToLower()))
            Subject.Reply(source, "Gem cannot be traded", "gem_trade_initial");
    }

    public void OnDisplayingGems(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var slot))
            return;

        var selectedItem = source.Inventory.FirstOrDefault(x => x.Slot == slot);

        if (selectedItem == null)
            return;

        foreach (var gemTemplateKey in GemTemplateKeys)
            if (!string.Equals(selectedItem.Template.TemplateKey, gemTemplateKey, StringComparison.CurrentCultureIgnoreCase))
            {
                var fauxEssenceItem = ItemFactory.CreateFaux(gemTemplateKey);
                Subject.Items.Add(ItemDetails.DisplayRecipe(fauxEssenceItem));
            }
    }

    public void OnDisplayingShowPlayerItems(Aisling source)
        => Subject.Slots = source.Inventory
                                 .Where(x => GemTemplateKeys.Contains(x.Template.TemplateKey.ToLower()))
                                 .Select(x => x.Slot)
                                 .ToList();

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.ToLower() == "new_gem_pick")
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
                Subject.Reply(source, "You ran out of those gems to trade.", "gem_trade_initial");

                return;
            }

            // Create the new essence item based on the player's selection.
            var gem = ItemFactory.CreateFaux(items.Template.TemplateKey);

            // Proceed to the confirmation dialog.
            Subject.Close(source);
            var dialog = DialogFactory.Create("gem_trade_confirmation", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(gem.DisplayName, item.DisplayName);
            dialog.Display(source);
        }
    }
}