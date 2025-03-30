using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class WeaponPolishingStoneScript : ItemScriptBase
{
    private readonly IDialogFactory DialogFactory;

    public WeaponPolishingStoneScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
        => DialogFactory = dialogFactory;

    public override void OnUse(Aisling source)
    {
        if (!source.UserStatSheet.Master)
        {
            source.SendOrangeBarMessage("You must be a master to use this stone.");

            return;
        }

        // Create an item dictionary with template keys (or other identifiers)
        var itemDictionary = new[]
        {
            "emeraldtonfa",
            "goodemeraldtonfa",
            "greatemeraldtonfa",
            "rubytonfa",
            "goodrubytonfa",
            "greatrubytonfa",
            "sapphiretonfa",
            "goodsapphiretonfa",
            "greatsapphiretonfa",
            "staffofaquaedon",
            "goodstaffofaquaedon",
            "greatstaffofaquaedon",
            "staffofzephyra",
            "goodstaffofzephyra",
            "greatstaffofzephyra",
            "staffofmiraelis",
            "goodstaffofmiraelis",
            "greatstaffofmiraelis",
            "chainwhip",
            "goodchainwhip",
            "greatchainwhip",
            "kris",
            "goodkris",
            "greatkris",
            "scurvydagger",
            "goodscurvydagger",
            "greatscurvydagger",
            "berylsaber",
            "goodberylsaber",
            "greatberylsaber",
            "rubysaber",
            "goodrubysaber",
            "greatrubysaber",
            "sapphiresaber",
            "goodsapphiresaber",
            "greatsapphiresaber",
            "forestescalon",
            "goodforestescalon",
            "greatforestescalon",
            "moonescalon",
            "goodmoonescalon",
            "greatmoonescalon",
            "shadowescalon",
            "goodshadowescalon",
            "greatshadowescalon",
            "staffofignatar",
            "goodstaffofignatar",
            "greatstaffofignatar",
            "staffoftheselene",
            "goodstaffoftheselene",
            "greatstaffoftheselene",
            "staffofgeolith",
            "goodstaffofgeolith",
            "greatstaffofgeolith",
            "nyxumbralshield",
            "goodnyxumbralshield",
            "greatnyxumbralshield"
        };

        // Check if any item in the dictionary is in the player's inventory
        var matchingItem = source.Inventory.FirstOrDefault(item => itemDictionary.ContainsI(item.Template.TemplateKey));

        if (matchingItem == null)

            // Send an orange bar message if no matching item is found
            source.SendOrangeBarMessage("You do not have a master item to upgrade.");
        else
        {
            // Create a dialog using the matching item
            var dialog = DialogFactory.Create("weaponpolishingstone_initial", matchingItem);
            dialog.Display(source);
        }
    }
}