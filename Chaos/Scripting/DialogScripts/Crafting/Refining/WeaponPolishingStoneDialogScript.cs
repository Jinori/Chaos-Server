using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.Refining;

public class WeaponPolishingStoneDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    public WeaponPolishingStoneDialogScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    private string GetNextUpgrade(string currentKey)
    {
        // Check current weapon and return next tier
        if (currentKey.StartsWith("grand", StringComparison.OrdinalIgnoreCase))
            return null!; // No upgrade beyond grand

        if (currentKey.StartsWith("great", StringComparison.OrdinalIgnoreCase))
            return "grand" + currentKey[5..]; // Upgrade from great to grand

        if (currentKey.StartsWith("good", StringComparison.OrdinalIgnoreCase))
            return "great" + currentKey[4..]; // Upgrade from good to great

        return "good" + currentKey; // Base to good
    }

    public override void OnDisplaying(Aisling source)
    {
        var upgradeableWeapons = new List<string>
        {
            "emeraldtonfa",
            "rubytonfa",
            "sapphiretonfa",
            "staffofaquaedon",
            "staffofzephyra",
            "staffofmiraelis",
            "chainwhip",
            "kris",
            "scurvydagger",
            "berylsaber",
            "rubysaber",
            "sapphiresaber",
            "forestescalon",
            "moonescalon",
            "shadowescalon",
            "staffofignatar",
            "staffoftheselene",
            "staffofgeolith",
            "nyxumbralshield"
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            
            case "weaponpolishingstone_initial":
            {
                // Find the first matching weapon in the player's inventory
                var weapon = source.Inventory.FirstOrDefault(
                    item => upgradeableWeapons.Any(baseKey => item.Template.TemplateKey.EndsWith(baseKey, StringComparison.OrdinalIgnoreCase)));
             
                if (weapon == null)
                {
                    source.SendOrangeBarMessage("You do not have a master item that can be upgraded.");
             
                    return;
                }
             
                // Handle upgrade based on current template key
                var currentKey = weapon.Template.TemplateKey.ToLower();
                Subject.InjectTextParameters(weapon.DisplayName, weapon.DisplayName);
                break;
            }
            
            case "weaponpolishingstone_polish":
            {
                var ani = new Animation
                     {
                         TargetAnimation = 7,
                         AnimationSpeed = 200,
                         Priority = 1
                     };
             
                     var failAnimation = new Animation
                     {
                         TargetAnimation = 51,
                         AnimationSpeed = 100,
                         Priority = 1
                     };
             
                     var failAnimation2 = new Animation
                     {
                         TargetAnimation = 107,
                         AnimationSpeed = 100,
                         Priority = 1
                     };
             
                     // Find the first matching weapon in the player's inventory
                     var weapon = source.Inventory.FirstOrDefault(
                         item => upgradeableWeapons.Any(baseKey => item.Template.TemplateKey.EndsWith(baseKey, StringComparison.OrdinalIgnoreCase)));
             
                     if (weapon == null)
                     {
                         source.SendOrangeBarMessage("You do not have a master item that can be upgraded.");
             
                         return;
                     }
             
                     // Handle upgrade based on current template key
                     var currentKey = weapon.Template.TemplateKey.ToLower();
                     var nextUpgrade = GetNextUpgrade(currentKey);
             
                     if (string.IsNullOrEmpty(nextUpgrade))
                     {
                         var itemName = ItemFactory.Create(currentKey);
                         source.SendOrangeBarMessage($"Your {itemName.DisplayName} cannot be upgraded further.");
             
                         return;
                     }
             
                     // Set fail rate based on the current upgrade level
                     var failChance = currentKey switch
                     {
                         _ when currentKey.StartsWith("good", StringComparison.OrdinalIgnoreCase)  => 70,
                         _ when currentKey.StartsWith("great", StringComparison.OrdinalIgnoreCase) => 90,
                         _                                                                         => 50 // 50% chance to fail for base weapons
                     };
             
                     // Check for failure
                     if (!IntegerRandomizer.RollChance(100 - failChance))
                     {
                         // Reduce weapon durability by 30%
                         var lossDurability = weapon.Template.MaxDurability * 0.3;
             
                         if (lossDurability > weapon.CurrentDurability)
                         {
                             source.Inventory.RemoveQuantityByTemplateKey(weapon.Template.TemplateKey, 1);
                             source.SendOrangeBarMessage($"The upgrade failed and your {weapon.DisplayName} broke!");
                             source.Inventory.RemoveQuantityByTemplateKey("polishingstone", 1);
             
                             source.Animate(failAnimation);
                             Subject.Close(source);
             
                             return;
                         }
             
                         var newDurability = weapon.CurrentDurability - lossDurability;
             
                         source.Inventory.Update(weapon.Slot, item => item.CurrentDurability = (int?)newDurability);
                         
                         source.SendOrangeBarMessage($"The upgrade failed! Your {weapon.DisplayName} lost durability.");
                         source.Inventory.RemoveQuantityByTemplateKey("polishingstone", 1);
             
                         source.Animate(failAnimation2);
             
                         Subject.Close(source);
             
                         return;
                     }
             
                     // Remove the old weapon and Polishing Stone
                     source.Inventory.RemoveQuantityByTemplateKey(currentKey, 1);
                     source.Inventory.RemoveQuantityByTemplateKey("polishingstone", 1);
             
                     // Create and give the upgraded weapon
                     var upgradedWeapon = ItemFactory.Create(nextUpgrade);
                     source.GiveItemOrSendToBank(upgradedWeapon);
                     source.SendOrangeBarMessage($"{upgradedWeapon.Template.Name} has been polished!");
                     Subject.InjectTextParameters(weapon.DisplayName, upgradedWeapon.Template.Name);
             
                     source.Animate(ani);
                 }
                break;
            }
        }
}