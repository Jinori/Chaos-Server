using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.DialogScripts.Generic;

public class DyeClassArmorsScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private Item? Item { get; set; }
    private MapInstance? MapInstance { get; set; }
    public InputCollector InputCollector { get; }

    public DyeClassArmorsScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;

        var requestOptionText = DialogString.From(() => $"I can dye your {Item?.DisplayName}. Would you like to continue?");

        InputCollector = new InputCollectorBuilder()
                         .RequestOptionSelection(requestOptionText, DialogString.Yes, DialogString.No)
                         .HandleInput(HandleInputOption)
                         .Build();
    }

    private bool HandleInputOption(Aisling source, Dialog dialog, int? option)
    {
        if (option is not 1)
            return false;

        if (Item is not null)
            switch (MapInstance?.InstanceId)
            {
                case "mileth_tailor":
                {
                    if (!source.Inventory.HasCount("Mileth Armor Dye", 1))
                    {
                        dialog.Close(source);
                        source.SendOrangeBarMessage("You have no Mileth Armor Dye, come back with it.");
                        return false;
                    }

                    var newArmor = ItemFactory.Create("mileth" + Item.Template.TemplateKey);
                    source.Inventory.Remove(Item.Template.Name);
                    source.Inventory.RemoveQuantity("Mileth Armor Dye", 1);
                    source.Inventory.TryAddToNextSlot(newArmor);
                    source.SendOrangeBarMessage($"You've successfully dyed your {Item.Template.Name}!");
                    dialog.Close(source);
                }

                    return true;

                case "rucesion_tailor":
                {
                    if (!source.Inventory.HasCount("Rucesion Armor Dye", 1))
                    {
                        dialog.Close(source);
                        source.SendOrangeBarMessage("You have no Rucesion Armor Dye, come back with it.");
                        return false;
                    }

                    var newArmor = ItemFactory.Create("rucesion" + Item.Template.TemplateKey);
                    source.Inventory.Remove(Item.Template.Name);
                    source.Inventory.RemoveQuantity("Rucesion Armor Dye", 1);
                    source.Inventory.TryAddToNextSlot(newArmor);
                    source.SendOrangeBarMessage($"You've successfully dyed your {Item.Template.Name}!");
                    dialog.Close(source);
                }

                    return true;

                case "suomi_armor_shop":
                {
                    if (!source.Inventory.HasCount("Suomi Armor Dye", 1))
                    {
                        dialog.Close(source);
                        source.SendOrangeBarMessage("You have no Suomi Armor Dye, come back with it.");
                        return false;
                    }

                    var newArmor = ItemFactory.Create("suomi" + Item.Template.TemplateKey);
                    source.Inventory.Remove(Item.Template.Name);
                    source.Inventory.RemoveQuantity("Suomi Armor Dye", 1);
                    source.Inventory.TryAddToNextSlot(newArmor);
                    source.SendOrangeBarMessage($"You've successfully dyed your {Item.Template.Name}!");
                    dialog.Close(source);
                }

                    return true;

                case "piet_storage":
                {
                    if (!source.Inventory.HasCount("Loures Armor Dye", 1))
                    {
                        dialog.Close(source);
                        source.SendOrangeBarMessage("You have no Loures Armor Dye, come back with it.");
                        return false;
                    }

                    var newArmor = ItemFactory.Create("loures" + Item.Template.TemplateKey);
                    source.Inventory.Remove(Item.Template.Name);
                    source.Inventory.RemoveQuantity("Loures Armor Dye", 1);
                    source.Inventory.TryAddToNextSlot(newArmor);
                    source.SendOrangeBarMessage($"You've successfully dyed your {Item.Template.Name}!");
                    dialog.Close(source);
                }

                    return true;
            }

        dialog.Close(source);
        source.SendOrangeBarMessage("You might be missing the armor dye required...");

        return false;
    }

    public override void OnDisplaying(Aisling source)
    {
        MapInstance = source.MapInstance;
        var inventory = source.Inventory.Where(x => x.Template.IsDyeable).ToList();

        if (Subject.Slots.IsNullOrEmpty())
            Subject.Slots = inventory.Select(x => x.Slot).ToList();
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