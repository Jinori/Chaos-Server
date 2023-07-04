using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class DyeClassArmorsScript : DialogScriptBase
{
    private readonly List<string> DyeAbleClassArmors = new()
    {
        "earthbodice", "lotusbodice", "moonbodice", "lightninggarb", "seagarb", "dobok", "culotte", "earthgarb", "windgarb", "mountaingarb",
        "gorgetgown", "mysticgown", "elle", "dolman", "bansagart", "cowl", "galuchatcoat", "mantle", "hierophant", "dalmatica",
        "cotte", "brigandine", "corsette", "pebblerose", "kagum", "scoutleather", "dwarvishleather", "paluten", "keaton", "bardocle",
        "leatherbliaut", "cuirass", "cotehardie", "kasmaniumhauberk", "labyrinthmail", "leathertunic", "jupe", "lorica", "chainmail",
        "platemail",
        "magiskirt", "benusta", "stoller", "clymouth", "clamyth", "gardcorp", "journeyman", "lorum", "mane", "duinuasal"
    };
    private readonly IItemFactory ItemFactory;

    public DyeClassArmorsScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

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

        var location = source.MapInstance.InstanceId switch
        {
            "mileth_tailor"    => "Mileth",
            "rucesion_tailor"  => "Rucesion",
            "suomi_armor_shop" => "Suomi",
            "piet_storage"     => "Loures",
            _                  => null
        };

        if (location == null)
        {
            Subject.Close(source);
            source.SendOrangeBarMessage("This location is not supported for armor dye.");

            return;
        }

        var armorDyeName = $"{location} Armor Dye";

        if (!source.Inventory.HasCount(armorDyeName, 1))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You have no {armorDyeName}, come back with it.");

            return;
        }

        var newArmor = ItemFactory.Create($"{location.ToLower()}{item.Template.TemplateKey}");
        source.Inventory.Remove(item.Template.Name);
        source.Inventory.RemoveQuantity(armorDyeName, 1);
        source.Inventory.TryAddToNextSlot(newArmor);
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

    private void OnDisplayingInitial(Aisling source) => Subject.Slots =
        source.Inventory.Where(x => DyeAbleClassArmors.Contains(x.Template.TemplateKey)).Select(x => x.Slot).ToList();
}