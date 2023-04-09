using Chaos.Containers;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class DyeClassArmorsScript : DialogScriptBase
{
    private string Location;
    private readonly IItemFactory ItemFactory;

    private readonly List<string> DyeAbleClassArmors = new List<string>()
    {
        "earthbodice", "lotusbodice", "moonbodice", "lightninggarb", "seagarb", "dobok", "culotte", "earthgarb", "windgarb", "mountaingarb",
        "gorgetgown", "mysticgown", "elle", "dolman", "bansagart", "cowl", "galuchatcoat", "mantle", "hierophant", "dalmatica",
        "cotte", "brigandine", "corsette", "pebblerose", "kagum", "scoutleather", "dwarvishleather", "paluten", "keaton", "bardocle",
        "leatherbliaut", "cuirass", "cotehardie", "kasmaniumhauberk", "labyrinthmail", "leathertunic", "jupe", "lorica", "chainmail", "platemail",
        "magiskirt", "benusta", "stoller", "clymouth", "clamyth", "gardcorp", "journeyman", "lorum", "mane", "duinuasal"
    };
    
    public DyeClassArmorsScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_dyeclassarmorinitial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_dyeclassarmorconfirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_dyeclassarmorfinish":
            {
                OnDisplayingAccepted(source);
                break;
            }   
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        Location = source.MapInstance.InstanceId switch
        {
            "mileth_tailor" => "Mileth",
            "rucesion_tailor" => "Rucesion",
            "suomi_armor_shop" => "Suomi",
            "piet_storage" => "Loures",
            _ => Location
        };
        
        switch (Location)
        {
            case "Mileth":
            {
                if (!source.Inventory.HasCount("Mileth Armor Dye", 1))
                {
                    Subject.Close(source);
                    source.SendOrangeBarMessage("You have no Mileth Armor Dye, come back with it.");
                    return;
                }

                var newArmor = ItemFactory.Create("mileth" + item.Template.TemplateKey);
                source.Inventory.Remove(item.Template.Name);
                source.Inventory.RemoveQuantity("Mileth Armor Dye", 1);
                source.Inventory.TryAddToNextSlot(newArmor);
                source.SendOrangeBarMessage($"You've successfully dyed your {item.Template.Name}!");
                break;
            }
            case "Rucesion":
            {
                if (!source.Inventory.HasCount("Rucesion Armor Dye", 1))
                {
                    Subject.Close(source);
                    source.SendOrangeBarMessage("You have no Rucesion Armor Dye, come back with it.");
                    return;
                }

                var newArmor = ItemFactory.Create("rucesion" + item.Template.TemplateKey);
                source.Inventory.Remove(item.Template.Name);
                source.Inventory.RemoveQuantity("Rucesion Armor Dye", 1);
                source.Inventory.TryAddToNextSlot(newArmor);
                source.SendOrangeBarMessage($"You've successfully dyed your {item.Template.Name}!");
                Subject.Close(source);
                break;
            }
            case "Suomi":
            {
                if (!source.Inventory.HasCount("Suomi Armor Dye", 1))
                {
                    Subject.Close(source);
                    source.SendOrangeBarMessage("You have no Suomi Armor Dye, come back with it.");
                    return;
                }

                var newArmor = ItemFactory.Create("suomi" + item.Template.TemplateKey);
                source.Inventory.Remove(item.Template.Name);
                source.Inventory.RemoveQuantity("Suomi Armor Dye", 1);
                source.Inventory.TryAddToNextSlot(newArmor);
                source.SendOrangeBarMessage($"You've successfully dyed your {item.Template.Name}!");
                Subject.Close(source);
                break;
            }
            case "Loures":
            {
                if (!source.Inventory.HasCount("Loures Armor Dye", 1))
                {
                    Subject.Close(source);
                    source.SendOrangeBarMessage("You have no Loures Armor Dye, come back with it.");
                    return;
                }

                var newArmor = ItemFactory.Create("loures" + item.Template.TemplateKey);
                source.Inventory.Remove(item.Template.Name);
                source.Inventory.RemoveQuantity("Loures Armor Dye", 1);
                source.Inventory.TryAddToNextSlot(newArmor);
                source.SendOrangeBarMessage($"You've successfully dyed your {item.Template.Name}!");
                Subject.Close(source);
                break;
            }
        }
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        Location = source.MapInstance.InstanceId switch
        {
            "mileth_tailor" => "Mileth",
            "rucesion_tailor" => "Rucesion",
            "suomi_armor_shop" => "Suomi",
            "piet_storage" => "Loures",
            _ => Location
        };

        Subject.InjectTextParameters(item.DisplayName, Location);
    }

    private void OnDisplayingInitial(Aisling source)
    {
        Subject.Slots = source.Inventory.Where(x => DyeAbleClassArmors.Contains(x.Template.TemplateKey)).Select(x => x.Slot).ToList();
    }
}