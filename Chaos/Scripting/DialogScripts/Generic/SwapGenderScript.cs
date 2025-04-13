using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class SwapGenderScript(Dialog subject, IItemFactory itemFactory, ILogger<SwapGenderScript> logger) : DialogScriptBase(subject)
{
    private const int STANDARD_PRICE = 50_000;
    private const int MASTER_PRICE = 1_000_000;

    public override void OnDisplaying(Aisling source)
    {
        var optionText = source.Gender == Gender.Male ? "Swap to Female" : "Swap to Male";

        if (!Subject.HasOption(optionText))
        {
            Subject.Options.Insert(0, new DialogOption
            {
                DialogKey = "josephine_swapgenderconfirm",
                OptionText = optionText
            });
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryPayGenderSwapCost(source))
            return;

        var fromGender = source.Gender;
        var toGender = fromGender == Gender.Male ? Gender.Female : Gender.Male;
        var baseClass = source.UserStatSheet.BaseClass;

        var swapMap = GetSwapMap(fromGender, baseClass);

        if (swapMap is null)
        {
            Subject.Reply(source, "Sorry, I couldn't find your class-specific equipment.");
            return;
        }

        SwapEquippedGear(source, swapMap, fromGender);
        SwapInventoryItems(source, swapMap);
        SwapBankItems(source, swapMap);
        ApplyGenderChange(source, toGender);
    }

    private bool TryPayGenderSwapCost(Aisling source)
    {
        var cost = source.UserStatSheet.Master ? MASTER_PRICE : STANDARD_PRICE;

        if (!source.TryTakeGold(cost))
        {
            Subject.Reply(source, "You don't have enough gold to do that.");
            
            logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold)
                  .WithProperty(this)
                  .LogInformation("Aisling {@AislingName} tried to buy a gender swap but didn't have {Amount} gold", source.Name, cost);
            
            return false;
        }
        
        logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold)
              .WithProperty(this)
              .LogInformation("Aisling {@AislingName} paid {Amount} gold for gender swap", source.Name, cost);
        
        return true;
    }

    private static Dictionary<string, string>? GetSwapMap(Gender fromGender, BaseClass baseClass) =>
        fromGender == Gender.Male
            ? GenderMasterEquipmentMap.MaleToFemale.GetValueOrDefault(baseClass)
            : GenderMasterEquipmentMap.FemaleToMale.GetValueOrDefault(baseClass);

    private void SwapEquippedGear(Aisling source, Dictionary<string, string> swapMap, Gender fromGender)
    {
        foreach (var equipped in source.Equipment.ToList())
        {
            var key = equipped.Template.TemplateKey.ToLower();

            if (swapMap.TryGetValue(key, out var replacementKey))
            {
                ReplaceItemInSlot(source, equipped.Slot, replacementKey, equipped.Template.Name);
            }
            else if (equipped.Template.Gender == fromGender)
            {
                source.Equipment.Remove(equipped.Slot);
                source.GiveItemOrSendToBank(equipped);
                source.SendMessage($"Your {equipped.Template.Name} was unequipped due to gender mismatch.");
                
                logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item)
                      .WithProperty(this)
                      .LogInformation("Aisling {@AislingName} unequipped {@ItemName} due to gender mismatch", source.Name, equipped.Template.Name);
            }
        }
    }

    private void SwapBankItems(Aisling source, Dictionary<string, string> swapMap)
    {
        foreach (var item in source.Bank.ToList())
        {
            var key = item.Template.TemplateKey.ToLower();

            if (swapMap.TryGetValue(key, out var replacementKey))
            {
                source.Bank.TryWithdraw(item.DisplayName, 1, out _);
                var replacement = itemFactory.Create(replacementKey);
                source.Bank.Deposit(replacement);

                source.SendMessage($"A {item.Template.Name} in your bank was gender swapped.");

                logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item)
                      .WithProperty(this)
                      .LogInformation(
                          "Aisling {@AislingName} swapped banked {@OldItem} for {@NewItem} via GenderSwap",
                          source.Name,
                          item.DisplayName,
                          replacement.DisplayName);
            }
        }
    }


    private void SwapInventoryItems(Aisling source, Dictionary<string, string> swapMap)
    {
        foreach (var item in source.Inventory.ToList())
        {
            var key = item.Template.TemplateKey.ToLower();

            if (swapMap.TryGetValue(key, out var replacementKey))
            {
                source.Inventory.Remove(item.Slot);
                var replacement = itemFactory.Create(replacementKey);
                source.GiveItemOrSendToBank(replacement);
                source.SendMessage($"Your inventory {item.Template.Name} was gender swapped.");
                
                logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item)
                      .WithProperty(this)
                      .LogInformation("Aisling {@AislingName} swapped their {@OldItem} for {@NewItem} via GenderSwap ", source.Name, item.DisplayName, replacement.DisplayName);
            }
        }
    }

    private void ReplaceItemInSlot(Aisling source, byte slot, string replacementKey, string oldItemName)
    {
        source.Equipment.Remove(slot);
        var replacement = itemFactory.Create(replacementKey);

        if (!source.Equipment.TryAdd(slot, replacement))
            source.GiveItemOrSendToBank(replacement);

        source.SendMessage($"Your equipped {oldItemName} was gender swapped.");
        
        logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item)
              .WithProperty(this)
              .LogInformation("Aisling {@AislingName} swapped their {@OldItem} for {@NewItem} via GenderSwap ", source.Name, oldItemName, replacement.DisplayName);
    }

    private void ApplyGenderChange(Aisling source, Gender toGender)
    {
        logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item)
              .WithProperty(this)
              .LogInformation("Aisling {@AislingName} swapped their gender from {@OldGender} to {@NewGender}", source.Name, source.Gender, toGender);
        
        source.Gender = toGender;
        source.BodySprite = toGender == Gender.Female ? BodySprite.Female : BodySprite.Male;

        source.Refresh(true);
        source.Display();
    }

    public static class GenderMasterEquipmentMap
    {
        public static readonly Dictionary<BaseClass, Dictionary<string, string>> MaleToFemale = new()
        {
            [BaseClass.Warrior] = new Dictionary<string, string>
            {
                { "warriormastermantle", "warriormasterdress" },
                { "refinedwarriormastermantle", "refinedwarriormasterdress" },
                { "malewarriormasterhelm", "femalewarriormasterhelm" },
                { "puremalegmwararmor", "purefemalegmwararmor" },
                { "submalegmwararmor", "subfemalegmwararmor" },
                { "puremalewarriorgmhelm", "purefemalewarriorgmhelm" },
                { "submalewarriorgmhelm", "subfemalewarriorgmhelm" }
            },
            [BaseClass.Monk] = new Dictionary<string, string>
            {
                { "monkmastermantle", "monkmasterdress" },
                { "refinedmonkmastermantle", "refinedmonkmasterdress" },
                { "malesunbaenimband", "femalesunbaenimband" },
                { "puremalegmdugon", "purefemalegmdugon" },
                { "submalegmdugon", "subfemalegmdugon" },
                { "puremalegmmonkarmor", "purefemalegmmonkarmor" },
                { "submalegmmonkarmor", "subfemalegmmonkarmor" }
            },
            [BaseClass.Priest] = new Dictionary<string, string>
            {
                { "sacredmantle", "sacreddress" },
                { "refinedsacredmantle", "refinedsacreddress" },
                { "sacredchief", "sacredwimple" },
                { "puregmdivinemitre", "puregmdivineband" },
                { "subgmcelestialmitre", "subgmcelestialband" },
                { "puremalegmpriestarmor", "purefemalegmpriestarmor" },
                { "submalegmpriestarmor", "subfemalegmpriestarmor" }
            },
            [BaseClass.Rogue] = new Dictionary<string, string>
            {
                { "roguemastermantle", "roguemasterdress" },
                { "refinedroguemastermantle", "refinedroguemasterdress" },
                { "puregmshadowmask", "puregmshadowhood" },
                { "subgmshadowmask", "subgmshadowhood" },
                { "puremalegmroguearmor", "purefemalegmroguearmor" },
                { "submalegmroguearmor", "subfemalegmroguearmor" }
            },
            [BaseClass.Wizard] = new Dictionary<string, string>
            {
                { "wizardmastermantle", "wizardmasterdress" },
                { "refinedwizardmastermantle", "refinedwizardmasterdress" },
                { "gnostichat", "gnosticumber" },
                { "puremalegmwizardhelm", "purefemalegmwizardhelm" },
                { "submalegmwizardhelm", "subfemalegmwizardhelm" },
                { "puremalegmwizardarmor", "purefemalegmwizardarmor" },
                { "submalegmwizardarmor", "subfemalegmwizardarmor" }
            },
        };

        public static readonly Dictionary<BaseClass, Dictionary<string, string>> FemaleToMale =
            MaleToFemale.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToDictionary(pair => pair.Value, pair => pair.Key)
            );
    }
}
