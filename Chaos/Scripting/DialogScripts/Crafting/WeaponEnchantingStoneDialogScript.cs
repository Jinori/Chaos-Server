using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class WeaponEnchantingStoneDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<WeaponEnchantingStoneDialogScript> Logger;

    public WeaponEnchantingStoneDialogScript(Dialog subject, IItemFactory itemFactory, ILogger<WeaponEnchantingStoneDialogScript> logger)
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
            AnimationSpeed = 200,
            Priority = 1
        };

        // Define the dictionary mapping master weapons to their enchanted versions
        var weaponEnchantments = new Dictionary<string, string>
        {
            {
                "kalkuri", "enchantedkalkuri"
            },
            {
                "holyhybrasylgnarl", "enchantedholygnarl"
            },
            {
                "hybrasylazoth", "enchantedhybrasylazoth"
            },
            {
                "magusorb", "enchantedmagusorb"
            },
            {
                "hybrasylescalon", "enchantedhybrasylescalon"
            }
        };

        // Search the player's inventory for a matching master weapon
        var masterWeapon = source.Inventory.FirstOrDefault(item => weaponEnchantments.ContainsKey(item.Template.TemplateKey.ToLower()));

        if (masterWeapon == null)
        {
            // Send a message if the player doesn't have a master weapon
            source.SendOrangeBarMessage("You do not have a master weapon that can be enchanted.");

            return;
        }

        // Get the corresponding enchanted weapon template key from the dictionary
        var enchantedWeaponTemplateKey = weaponEnchantments[masterWeapon.Template.TemplateKey.ToLower()];

        // Remove the old master weapon from the player's inventory
        source.Inventory.RemoveByTemplateKey(masterWeapon.Template.TemplateKey.ToLower());
        source.Inventory.Remove("Enchanting Stone");

        // Create the new enchanted weapon
        var enchantedWeapon = ItemFactory.Create(enchantedWeaponTemplateKey.ToLower());

        // Give the player the enchanted weapon
        source.GiveItemOrSendToBank(enchantedWeapon);
        source.Animate(ani);

        // Send a message to the player
        source.SendOrangeBarMessage($"Your weapon has been enchanted into a {enchantedWeapon.Template.Name}!");
    }
}