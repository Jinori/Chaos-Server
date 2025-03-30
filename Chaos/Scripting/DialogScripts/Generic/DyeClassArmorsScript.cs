using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class DyeClassArmorsScript : DialogScriptBase
{
    private readonly List<string> DyeAbleClassArmors =
    [
        "earthbodice",
        "lotusbodice",
        "moonbodice",
        "lightninggarb",
        "seagarb",
        "dobok",
        "culotte",
        "earthgarb",
        "windgarb",
        "mountaingarb",
        "gorgetgown",
        "mysticgown",
        "elle",
        "dolman",
        "bansagart",
        "cowl",
        "galuchatcoat",
        "mantle",
        "hierophant",
        "dalmatica",
        "cotte",
        "brigandine",
        "corsette",
        "pebblerose",
        "kagum",
        "scoutleather",
        "dwarvishleather",
        "paluten",
        "keaton",
        "bardocle",
        "leatherbliaut",
        "cuirass",
        "cotehardie",
        "kasmaniumhauberk",
        "labyrinthmail",
        "leathertunic",
        "jupe",
        "lorica",
        "chainmail",
        "platemail",
        "platemailhelmet",
        "magiskirt",
        "benusta",
        "stoller",
        "clymouth",
        "clamyth",
        "gardcorp",
        "journeyman",
        "lorum",
        "mane",
        "duinuasal"
    ];

    private readonly IItemFactory ItemFactory;

    public DyeClassArmorsScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

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

        // Map town locations to both specific and generic dye names
        var dyeMapping = new Dictionary<string, (string specificDye, string genericDye)>
        {
            {
                "Mileth", ("Mileth Armor Dye", "Green Armor Dye")
            },
            {
                "Rucesion", ("Rucesion Armor Dye", "Blue Armor Dye")
            },
            {
                "Suomi", ("Suomi Armor Dye", "Apple Armor Dye")
            },
            {
                "Loures", ("Loures Armor Dye", "White Armor Dye")
            }
        };

        // Identify the current location and get the associated dye names
        var location = source.MapInstance.InstanceId switch
        {
            "mileth_tailor"    => "Mileth",
            "rucesion_tailor"  => "Rucesion",
            "suomi_armor_shop" => "Suomi",
            "piet_storage"     => "Loures",
            _                  => null
        };

        if ((location == null) || !dyeMapping.TryGetValue(location, out var value))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage("This location is not supported for armor dye.");

            return;
        }

        // Get the specific and generic dye names from the mapping
        (var specificDye, var genericDye) = value;

        // Check for both specific and generic dyes
        if (!source.Inventory.HasCount(specificDye, 1) && !source.Inventory.HasCount(genericDye, 1))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You have no {specificDye} or {genericDye}, come back with either one.");

            return;
        }

        // Create the dyed version of the item using the location and the item template
        var newArmor = ItemFactory.Create($"{location.ToLower()}{item.Template.TemplateKey}");
        source.Inventory.RemoveByTemplateKey(item.Template.TemplateKey);

        // Remove the dye from inventory (prioritize specific dye, fallback to generic)
        if (source.Inventory.HasCount(specificDye, 1))
            source.Inventory.RemoveQuantity(specificDye, 1);
        else
            source.Inventory.RemoveQuantity(genericDye, 1);

        source.GiveItemOrSendToBank(newArmor);
        source.SendOrangeBarMessage($"You've successfully dyed your {item.Template.Name}!");
        Subject.Close(source);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var location = "";

        location = source.MapInstance.InstanceId switch
        {
            "mileth_tailor"    => "Mileth",
            "rucesion_tailor"  => "Rucesion",
            "suomi_armor_shop" => "Suomi",
            "piet_storage"     => "Loures",
            _                  => location
        };

        Subject.InjectTextParameters(item.DisplayName, location);
    }

    private void OnDisplayingInitial(Aisling source)
        => Subject.Slots = source.Inventory
                                 .Where(x => DyeAbleClassArmors.Contains(x.Template.TemplateKey))
                                 .Select(x => x.Slot)
                                 .ToList();
}