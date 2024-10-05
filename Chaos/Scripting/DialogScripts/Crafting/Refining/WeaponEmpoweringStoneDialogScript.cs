using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Crafting.Refining;

public class WeaponEmpoweringStoneDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<WeaponEnchantingStoneDialogScript> Logger;

    public WeaponEmpoweringStoneDialogScript(Dialog subject, IItemFactory itemFactory,
        ILogger<WeaponEnchantingStoneDialogScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
    }
    
    public override void OnDisplaying(Aisling source)
    { 
        var ani = new Animation
    {
        TargetAnimation = 7,
        AnimationSpeed = 1,
        Priority = 1,
    };
        // Define the dictionary mapping master weapons to their enchanted versions
        var weaponEmpowering = new Dictionary<string, string>
        {
            { "kalkuri", "empoweredkalkuri" },
            { "holyhybrasylgnarl", "empoweredholygnarl" },
            { "hybrasylazoth", "empoweredhybrasylazoth" },
            { "magusorb", "empoweredmagusorb" },
            { "hybrasylescalon", "empoweredescalon" },
        };

        // Search the player's inventory for a matching master weapon
        var masterWeapon = source.Inventory.FirstOrDefault(item =>
            weaponEmpowering.ContainsKey(item.Template.TemplateKey));

        if (masterWeapon == null)
        {
            // Send a message if the player doesn't have a master weapon
            source.SendOrangeBarMessage("You do not have a master weapon that can be empowered.");
            return;
        }

        // Get the corresponding enchanted weapon template key from the dictionary
        var empoweredWeaponTemplateKey = weaponEmpowering[masterWeapon.Template.TemplateKey];

        // Remove the old master weapon from the player's inventory
        source.Inventory.Remove(masterWeapon.Template.TemplateKey);

        // Create the new enchanted weapon
        var empowerWeapon = ItemFactory.Create(empoweredWeaponTemplateKey);
        
        // Give the player the enchanted weapon
        source.GiveItemOrSendToBank(empowerWeapon);
        source.Animate(ani);

        // Send a message to the player
        source.SendOrangeBarMessage($"Your {masterWeapon.Template.Name} has been enchanted into a {empowerWeapon.Template.Name}!");
    }
}
