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

public class WeaponEnchantingStoneDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<WeaponEnchantingStoneDialogScript> Logger;

    public WeaponEnchantingStoneDialogScript(Dialog subject, IItemFactory itemFactory,
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
        var weaponEnchantments = new Dictionary<string, string>
        {
            { "kalkuri", "enchantedkalkuri" },
            { "holyhybrasylgnarl", "enchantedholygnarl" },
            { "hybrasylazoth", "enchantedhybrasylazoth" },
            { "magusorb", "enchantedmagusorb" },
            { "hybrasylescalon", "enchantedescalon" },
        };

        // Search the player's inventory for a matching master weapon
        var masterWeapon = source.Inventory.FirstOrDefault(item =>
            weaponEnchantments.ContainsKey(item.Template.TemplateKey));

        if (masterWeapon == null)
        {
            // Send a message if the player doesn't have a master weapon
            source.SendOrangeBarMessage("You do not have a master weapon that can be enchanted.");
            return;
        }

        // Get the corresponding enchanted weapon template key from the dictionary
        var enchantedWeaponTemplateKey = weaponEnchantments[masterWeapon.Template.TemplateKey];

        // Remove the old master weapon from the player's inventory
        source.Inventory.Remove(masterWeapon.Template.TemplateKey);

        // Create the new enchanted weapon
        var enchantedWeapon = ItemFactory.Create(enchantedWeaponTemplateKey);
        
        // Give the player the enchanted weapon
        source.GiveItemOrSendToBank(enchantedWeapon);
        source.Animate(ani);

        // Send a message to the player
        source.SendOrangeBarMessage($"Your {masterWeapon.Template.Name} has been enchanted into a {enchantedWeapon.Template.Name}!");
    }
}
