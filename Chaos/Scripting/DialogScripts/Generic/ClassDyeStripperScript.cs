using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class ClassDyeStripperScript(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
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

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_dyestripperinitial":
                OnDisplayingInitial(source);

                break;
            case "generic_dyestripperconfirmation":
                OnDisplayingConfirmation(source);

                break;
            case "generic_dyestripperfinish":
                OnDisplayingAccepted(source);

                break;
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        // Check if the player has the Dye Stripper
        const string DYE_STRIPPER_ITEM = "Dye Stripper";

        if (!source.Inventory.HasCount(DYE_STRIPPER_ITEM, 1))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage($"You have no {DYE_STRIPPER_ITEM}, come back with one.");

            return;
        }

        // Verify if the item is dyeable (check for any of the town prefixes)
        if (!(item.Template.TemplateKey.StartsWithI("mileth")
              || item.Template.TemplateKey.StartsWithI("rucesion")
              || item.Template.TemplateKey.StartsWithI("loures")
              || item.Template.TemplateKey.StartsWithI("suomi")))
        {
            Subject.Close(source);
            source.SendOrangeBarMessage("This item cannot have its dye stripped.");

            return;
        }

        // Define dye prefixes for removal
        var dyePrefixes = new List<string>
        {
            "mileth",
            "rucesion",
            "loures",
            "suomi"
        };

        foreach (var prefix in dyePrefixes)
            if (item.Template.TemplateKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                // Remove the prefix from the template key
                var originalTemplateKey = item.Template.TemplateKey.Substring(prefix.Length);

                // Check if the original template is in the DyeAbleClassArmors list
                if (!DyeAbleClassArmors.Contains(originalTemplateKey))
                {
                    source.SendOrangeBarMessage("The original armor is not valid.");

                    return;
                }

                // Create the original armor (without the dye prefix)
                var originalArmor = itemFactory.Create(originalTemplateKey);

                // Add the original armor to the player's inventory
                source.GiveItemOrSendToBank(originalArmor);

                // Remove the dyed armor and the dye stripper from inventory
                source.Inventory.Remove(item.DisplayName);
                source.Inventory.RemoveQuantity(DYE_STRIPPER_ITEM, 1);

                source.SendOrangeBarMessage($"You've removed the dye from your {item.DisplayName}!");

                break;
            }

        Subject.Close(source);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName);
    }

    private void OnDisplayingInitial(Aisling source)
        => Subject.Slots = source.Inventory
                                 .Where(
                                     x => x.Template.TemplateKey.StartsWithI("mileth")
                                          || x.Template.TemplateKey.StartsWithI("rucesion")
                                          || x.Template.TemplateKey.StartsWithI("loures")
                                          || x.Template.TemplateKey.StartsWithI("suomi"))
                                 .Select(x => x.Slot)
                                 .ToList();
}