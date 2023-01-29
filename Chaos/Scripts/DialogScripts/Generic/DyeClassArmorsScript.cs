using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts.Generic;

public class DyeClassArmorsScript : DialogScriptBase
{
    private readonly ILogger<DepositItemScript> Logger;
    private int? Amount { get; set; }
    private Item? Item { get; set; }
    public InputCollector InputCollector { get; }
    private MapInstance MapInstance { get; set; }
    private string DyeColor { get; set; }
    
    private readonly IItemFactory ItemFactory;
    
    public DyeClassArmorsScript(Dialog subject, ILogger<DepositItemScript> logger, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        
        switch (MapInstance.InstanceId)
        {
            case "rucesion_tailor":
                DyeColor = "Rucesion Armor Dye";
                break;
            
            case "mileth_tailor":
                DyeColor = "Mileth Armor Dye";
                break;
            
            case "suomi_armor_shop":
                DyeColor = "Suomi Armor Dye";
                break;
            
            case "piet_storage":
                DyeColor = "Loures Armor Dye";
                break;
        }

        var requestOptionText = DialogString.From(
            () => $"I can dye your armor using {DyeColor}. Would you like to continue?");
        
        
        InputCollector = new InputCollectorBuilder()
                         .RequestOptionSelection(requestOptionText, DialogString.Yes, DialogString.No)
                         .HandleInput(HandleInputOption)
                         .Build();
    }
    

    private bool HandleInputOption(Aisling source, Dialog dialog, int? option)
    {
        if (option is not 1)
            return false;

        if (Item.Template.Name.EqualsI("leathertunic"))
        {
            if (DyeColor.EqualsI("Rucesion Armor Dye") && source.Inventory.HasCount(DyeColor, 1))
            {
                var newArmor = ItemFactory.Create("rucesionleathertunic");
                source.Inventory.Remove("Leather Tunic");
                source.Inventory.TryAddToNextSlot(newArmor);
            }

            if (DyeColor.EqualsI("Mileth Armor Dye") && source.Inventory.HasCount(DyeColor, 1))
            {
                var newArmor = ItemFactory.Create("milethleathertunic");
                source.Inventory.Remove("Leather Tunic");
                source.Inventory.TryAddToNextSlot(newArmor);
            }

            if (DyeColor.EqualsI("Suomi Armor Dye") && source.Inventory.HasCount(DyeColor, 1))
            {
                var newArmor = ItemFactory.Create("suomileathertunic");
                source.Inventory.Remove("Leather Tunic");
                source.Inventory.TryAddToNextSlot(newArmor);
            }
            
            if (DyeColor.EqualsI("Loures Armor Dye") && source.Inventory.HasCount(DyeColor, 1))
            {
                var newArmor = ItemFactory.Create("louresleathertunic");
                source.Inventory.Remove("Leather Tunic");
                source.Inventory.TryAddToNextSlot(newArmor);
            }
        }
        
        
        source.SendOrangeBarMessage("You've successfully dyed your amor!");
        return true;
    }

    public override void OnDisplaying(Aisling source)
    {
        MapInstance = source.MapInstance;
        
        if (Subject.Slots.IsNullOrEmpty())
        {
            foreach (var item in source.Inventory)
            {
                if (item.Template.IsDyeable)
                    Subject.Slots.Add(item.Slot);
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Item == null)
        {
            if (!Subject.MenuArgs.TryGet<byte>(0, out var slot))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }

            var item = source.Inventory[slot];

            if (item == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }

            Item = item;
        }
        
        InputCollector.Collect(source, Subject, optionIndex);
    }
}