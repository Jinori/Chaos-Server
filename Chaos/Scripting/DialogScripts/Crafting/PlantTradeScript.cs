using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class PlantTradeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;

    private readonly List<string> PlantTemplateKeys = new()
    {
        "waterlily", "petunia", "dochasbloom", "lilypad", "kabineblossom", "cactusflower", "koboldtail"
    };

    /// <inheritdoc />
    public PlantTradeScript(
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
            case "plant_trade_initial":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "plant_trade_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "plant_trade_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
            case "new_plant_pick":
            {
                OnDisplayingPlants(source);

                break;
            }
        }
    }

    public void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, $"You ran out of those plants to trade.", "plant_trade_initial");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 1);

        if (!Subject.MenuArgs.TryGet<string>(1, out var previous))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        
        var newPlant = ItemFactory.Create(previous.ToLowerInvariant().Replace(" ", string.Empty));
        
        if (!source.CanCarry(newPlant))
        {
            source.Bank.Deposit(newPlant);
            source.SendOrangeBarMessage($"{newPlant.DisplayName} was sent to your bank.");
        }
        else
            source.Inventory.TryAddToNextSlot(newPlant);

        Subject.InjectTextParameters(item.DisplayName, newPlant.DisplayName);
    }

    public void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, "Skip", "plant_trade_initial");

            return;
        }

        if (!PlantTemplateKeys.Contains(item.Template.TemplateKey.ToLower()))
            Subject.Reply(source, "Item cannot be refined", "plant_trade_initial");
    }

    public void OnDisplayingShowPlayerItems(Aisling source) => Subject.Slots =
        source.Inventory.Where(x => PlantTemplateKeys.Contains(x.Template.TemplateKey.ToLower()))
              .Select(x => x.Slot)
              .ToList();

    public void OnDisplayingPlants(Aisling source)
    {
        foreach (var plantTemplateKey in PlantTemplateKeys)
        {
            var item = ItemFactory.CreateFaux(plantTemplateKey);
            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
        }
    }


    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.ToLower() == "new_plant_pick")
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

            // Find the slot of the plant item the player wants to trade.
            if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
            {
                Subject.Reply(source, $"You ran out of those plants to trade.", "plant_trade_initial");

                return;
            }

            // Create the new plant item based on the player's selection.
            var plant = ItemFactory.CreateFaux(items.Template.TemplateKey);

            // Proceed to the confirmation dialog.
            Subject.Close(source);
            var dialog = DialogFactory.Create("plant_trade_confirmation", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(item.DisplayName, plant.DisplayName);
            dialog.Display(source);   
        }
    }
}