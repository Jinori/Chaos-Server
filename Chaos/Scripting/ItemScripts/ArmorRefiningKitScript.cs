using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class ArmorRefiningKitScript : ItemScriptBase
{
    private readonly IDialogFactory DialogFactory;
    
    public ArmorRefiningKitScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
    }

    public override void OnUse(Aisling source)
    {
        if (!source.UserStatSheet.Master)
        {
            source.SendOrangeBarMessage("You must be a master to use this kit.");
            return;
        }
        
        // Create an item dictionary with template keys (or other identifiers)
        var itemDictionary = new[] 
        {
            "monkmasterdress",  // Replace with actual template keys
            "monkmastermantle",
            "sacreddress",
            "sacredmantle",
            "roguemasterdress",
            "roguemastermantle",
            "warriormasterdress",
            "warriormastermantle",
            "wizardmasterdress",
            "wizardmastermantle",
        };

        // Check if any item in the dictionary is in the player's inventory
        var matchingItem = source.Inventory.FirstOrDefault(item => 
            itemDictionary.ContainsI(item.Template.TemplateKey));

        if (matchingItem == null)
        {
            // Send an orange bar message if no matching item is found
            source.SendOrangeBarMessage("You do not have Master Armor to refine.");
        }
        else
        {
            // Create a dialog using the matching item
            var dialog = DialogFactory.Create("armorrefiningkit_initial", matchingItem);
            dialog.Display(source);
        }
    }
}