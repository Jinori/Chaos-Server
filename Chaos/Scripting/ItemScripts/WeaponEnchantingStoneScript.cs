using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class WeaponEnchantingStoneScript : ItemScriptBase
{
    private readonly IDialogFactory DialogFactory;
    
    public WeaponEnchantingStoneScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
    }

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
            "kalkuri",  // Replace with actual template keys
            "holyhybrasylgnarl",
            "hybrasylazoth",
            "hybrasylescalon",
            "magusorb"
        };

        // Check if any item in the dictionary is in the player's inventory
        var matchingItem = source.Inventory.FirstOrDefault(item => 
            itemDictionary.Contains(item.Template.TemplateKey));

        if (matchingItem == null)
        {
            // Send an orange bar message if no matching item is found
            source.SendOrangeBarMessage("You do not have a master weapon to enchant.");
        }
        else
        {
            // Create a dialog using the matching item
            var dialog = DialogFactory.Create("weaponenchantingstone_initial", matchingItem);
            dialog.Display(source);
        }
    }
}