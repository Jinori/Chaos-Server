using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.Refining;

public class ArmorsmithingKitDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<WeaponEnchantingStoneDialogScript> Logger;

    public ArmorsmithingKitDialogScript(Dialog subject, IItemFactory itemFactory, ILogger<WeaponEnchantingStoneDialogScript> logger)
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
        var armorRefining = new Dictionary<string, string>
        {
            {
                "monkmasterdress", "refinedmonkmasterdress"
            },
            {
                "monkmastermantle", "refinedmonkmastermantle"
            },
            {
                "sacreddress", "refinedsacreddress"
            },
            {
                "sacredmantle", "refinedsacredmantle"
            },
            {
                "roguemasterdress", "refinedroguemasterdress"
            },
            {
                "roguemastermantle", "refinedroguemastermantle"
            },
            {
                "warriormasterdress", "refinedwarriormasterdress"
            },
            {
                "warriormastermantle", "refinedwarriormastermantle"
            },
            {
                "wizardmasterdress", "refinedwizardmasterdress"
            },
            {
                "wizardmastermantle", "refinedwizardmastermantle"
            }
        };

        // Search the player's inventory for a matching master weapon
        var masterarmor = source.Inventory.FirstOrDefault(item => armorRefining.ContainsKey(item.Template.TemplateKey.ToLower()));

        if (masterarmor == null)
        {
            // Send a message if the player doesn't have a master weapon
            source.SendOrangeBarMessage("You do not have a master armor that can be refined.");

            return;
        }

        // Get the corresponding enchanted weapon template key from the dictionary
        var refinedArmorTemplateKey = armorRefining[masterarmor.Template.TemplateKey.ToLower()];

        // Remove the old master weapon from the player's inventory
        source.Inventory.RemoveByTemplateKey(masterarmor.Template.TemplateKey.ToLower());
        source.Inventory.Remove("Refining Kit");

        // Create the new enchanted weapon
        var refinedArmor = ItemFactory.Create(refinedArmorTemplateKey.ToLower());

        // Give the player the enchanted weapon
        source.GiveItemOrSendToBank(refinedArmor);
        source.Animate(ani);

        // Send a message to the player
        source.SendOrangeBarMessage($"Your armor has been refined to {refinedArmor.Template.Name}!");
    }
}