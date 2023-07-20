using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class PlantTradeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;
    private Item? NewPlant { get; set; }

    private readonly List<string> PlantTemplateKeys = new()
    {
        "waterlily", "petunia", "dochasbloom", "lilypad", "kabineblossom", "cactusflower", "squirtweed", "koboldtail"
    };

    /// <inheritdoc />
    public PlantTradeScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory,
        Item newPlant
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

        if (!source.CanCarry(NewPlant))
        {
            source.Bank.Deposit(NewPlant);
            source.SendOrangeBarMessage($"{NewPlant.DisplayName} was sent to your bank.");
        } else
            source.Inventory.TryAddToNextSlot(NewPlant);

        Subject.InjectTextParameters(item.DisplayName, NewPlant.DisplayName);
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
        Subject.Items.Clear(); // Clear the existing items to show the plant options.

        foreach (var plantTemplateKey in PlantTemplateKeys)
        {
            var item = ItemFactory.CreateFaux(plantTemplateKey);
            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
        }

        Subject.Reply(source, "Choose a plant.", "new_plant_pick");
    }


    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex.HasValue)
        {
            // Get the selected plant item from the list.
            var selectedPlantItem = PlantTemplateKeys.ElementAtOrDefault(optionIndex.Value);

            // Ensure the selection is valid.
            if (string.IsNullOrEmpty(selectedPlantItem))
            {
                source.SendActiveMessage("Invalid selection.");
                return;
            }

            // Find the slot of the plant item the player wants to trade.
            if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
            {
                Subject.Reply(source, $"You ran out of those plants to trade.", "plant_trade_initial");
                return;
            }
            
            // Create the new plant item based on the player's selection.
            NewPlant = ItemFactory.CreateFaux(selectedPlantItem);

            // Proceed to the confirmation dialog.
            Subject.Close(source);
            var dialog = DialogFactory.Create("plant_trade_confirmation", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(item.DisplayName, NewPlant.DisplayName);
            dialog.Display(source);
        }
        else
        {
            Subject.Reply(source, "That plant isn't available.", "new_plant_pick");
        }
    }


}