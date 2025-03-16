using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class MasteringScript : DialogScriptBase
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<MasteringScript> Logger;

    private readonly Dictionary<BaseClass, (string itemOne, string itemTwo)> MasteringItemRequirements = new()
    {
        {
            BaseClass.Monk, ("goldsand", "cauldron")
        },
        {
            BaseClass.Warrior, ("brokenjackalsword", "jackalhilt")
        },
        {
            BaseClass.Rogue, ("copperfile", "smithhammer")
        },
        {
            BaseClass.Wizard, ("magicink", "magicscroll")
        },
        {
            BaseClass.Priest, ("holyink", "holyscroll")
        }
    };

    private readonly Dictionary<BaseClass, (int requiredHealth, int requiredMana)> MasteringRequirements = new()
    {
        {
            BaseClass.Monk, (16000, 7000)
        },
        {
            BaseClass.Warrior, (15000, 7500)
        },
        {
            BaseClass.Rogue, (14000, 8000)
        },
        {
            BaseClass.Wizard, (10000, 12000)
        },
        {
            BaseClass.Priest, (9500, 13000)
        }
    };

    private readonly Dictionary<BaseClass, (string skillOne, string skillTwo)> MasteringSkillSpellRequirements = new()
    {
        {
            BaseClass.Monk, ("Wolf Fang Fist", "Rapid Punch")
        },
        {
            BaseClass.Warrior, ("Crasher", "Pulverize")
        },
        {
            BaseClass.Rogue, ("Skewer", "Maiden Trap")
        },
        {
            BaseClass.Wizard, ("Mor Cradh", "Arcane Blast")
        },
        {
            BaseClass.Priest, ("Ard Ioc", "beag pramh")
        }
    };

    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public MasteringScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IItemFactory itemFactory,
        ILogger<MasteringScript> logger,
        ISpellFactory spellFactory,
        ISkillFactory skillFactory)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        ItemFactory = itemFactory;
        Logger = logger;
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
    }

    private void AwardClassSpecificItemsAndAbilities(Aisling source, BaseClass userBaseClass)
    {
        switch (userBaseClass)
        {
            case BaseClass.Warrior:
                AwardWarriorItemsAndAbilities(source);

                break;
            case BaseClass.Rogue:
                AwardRogueItemsAndAbilities(source);

                break;
            case BaseClass.Wizard:
                AwardWizardItemsAndAbilities(source);

                break;
            case BaseClass.Priest:
                AwardPriestItemsAndAbilities(source);

                break;
            case BaseClass.Monk:
                AwardMonkItemsAndAbilities(source);

                break;
        }
    }

    private void AwardMasterLegendMark(Aisling source, BaseClass baseClass, string classKey)
        => source.Legend.AddOrAccumulate(
            new LegendMark(
                $"Wields the Mantle of {baseClass.ToString()} Master",
                classKey,
                MarkIcon.Victory,
                MarkColor.Blue,
                1,
                GameTime.Now));

    private void AwardMonkItemsAndAbilities(Aisling source)
    {
        var armor = source.Gender == Gender.Male ? ItemFactory.Create("monkmastermantle") : ItemFactory.Create("monkmasterdress");
        var helm = source.Gender == Gender.Male ? ItemFactory.Create("maleSunBaeNimBand") : ItemFactory.Create("femaleSunBaeNimBand");
        var weapon = ItemFactory.Create("kalkuri");
        var triplekick = SkillFactory.Create("triplekick");
        var roar = SpellFactory.Create("roar");
        var adaptiveskin = SpellFactory.Create("adaptiveskin");

        source.SkillBook.Remove("Triple Kick");
        source.SpellBook.Remove("Howl");

        source.SkillBook.TryAddToNextSlot(triplekick);
        source.SpellBook.TryAddToNextSlot(roar);
        source.SpellBook.TryAddToNextSlot(adaptiveskin);

        if (!source.Legend.TryGetValue("dedicated", out _))
        {
            var phoenixgrasp = SpellFactory.Create("phoenixgrasp");
            source.SpellBook.TryAddToNextSlot(phoenixgrasp);
        }

        var itemsToGive = new[]
        {
            armor,
            helm,
            weapon
        };

        foreach (var item in itemsToGive)
            source.GiveItemOrSendToBank(item);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Create)
              .WithProperty(Subject)
              .WithProperty(itemsToGive)
              .LogInformation(
                  "{@AislingName} has received {@Armor}, {@Helm}, {@Weapon} from mastering",
                  source.Name,
                  armor.DisplayName,
                  helm.DisplayName,
                  weapon.DisplayName);
    }

    private void AwardPriestItemsAndAbilities(Aisling source)
    {
        var armor = source.Gender == Gender.Male ? ItemFactory.Create("sacredmantle") : ItemFactory.Create("sacreddress");
        var helm = source.Gender == Gender.Male ? ItemFactory.Create("sacredchief") : ItemFactory.Create("sacredwimple");
        var weapon = ItemFactory.Create("holyhybrasylgnarl");
        var pramh = SpellFactory.Create("pramh");
        var vortex = SpellFactory.Create("vortex");

        source.SpellBook.Remove("beag pramh");
        source.SpellBook.Remove("quake");

        source.SpellBook.TryAddToNextSlot(pramh);
        source.SpellBook.TryAddToNextSlot(vortex);

        if (!source.Legend.TryGetValue("dedicated", out _))
        {
            source.SpellBook.Remove("revive");
            var resurrect = SpellFactory.Create("resurrect");
            source.SpellBook.TryAddToNextSlot(resurrect);
        }

        var itemsToGive = new[]
        {
            armor,
            helm,
            weapon
        };

        foreach (var item in itemsToGive)
            source.GiveItemOrSendToBank(item);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Create)
              .WithProperty(Subject)
              .WithProperty(itemsToGive)
              .LogInformation(
                  "{@AislingName} has received {@Armor}, {@Helm}, {@Weapon} from mastering",
                  source.Name,
                  armor.DisplayName,
                  helm.DisplayName,
                  weapon.DisplayName);
    }

    private void AwardRogueItemsAndAbilities(Aisling source)
    {
        var armor = source.Gender == Gender.Male ? ItemFactory.Create("rogueMasterMantle") : ItemFactory.Create("rogueMasterdress");
        var helm = ItemFactory.Create("shadowMask");
        var weapon = ItemFactory.Create("hybrasylazoth");
        var midnightslash = SkillFactory.Create("midnightslash");
        var assassinstrike = SkillFactory.Create("assassinstrike");

        source.SkillBook.TryAddToNextSlot(midnightslash);
        source.SkillBook.TryAddToNextSlot(assassinstrike);

        if (!source.Legend.TryGetValue("dedicated", out _))
        {
            var shadowfigure = SkillFactory.Create("shadowfigure");
            source.SkillBook.TryAddToNextSlot(shadowfigure);
            source.SkillBook.RemoveByTemplateKey("backstab");
        }

        var itemsToGive = new[]
        {
            armor,
            helm,
            weapon
        };

        foreach (var item in itemsToGive)
            source.GiveItemOrSendToBank(item);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Create)
              .WithProperty(Subject)
              .WithProperty(itemsToGive)
              .LogInformation(
                  "{@AislingName} has received {@Armor}, {@Helm}, {@Weapon} from mastering",
                  source.Name,
                  armor.DisplayName,
                  helm.DisplayName,
                  weapon.DisplayName);
    }

    private void AwardWarriorItemsAndAbilities(Aisling source)
    {
        var armor = source.Gender == Gender.Male ? ItemFactory.Create("warriormastermantle") : ItemFactory.Create("warriormasterdress");

        var helm = source.Gender == Gender.Male
            ? ItemFactory.Create("malewarriormasterhelm")
            : ItemFactory.Create("femalewarriormasterhelm");
        var weapon = ItemFactory.Create("hybrasylescalon");
        var thrash = SkillFactory.Create("thrash");
        var charge = SkillFactory.Create("charge");

        source.SkillBook.Remove("Bull Rush");
        source.SkillBook.Remove("Pulverize");

        source.SkillBook.TryAddToNextSlot(thrash);
        source.SkillBook.TryAddToNextSlot(charge);

        if (!source.Legend.TryGetValue("dedicated", out _))
        {
            var asgallfaileas = SpellFactory.Create("asgallfaileas");
            source.SpellBook.TryAddToNextSlot(asgallfaileas);
        }

        var itemsToGive = new[]
        {
            armor,
            helm,
            weapon
        };

        foreach (var item in itemsToGive)
            source.GiveItemOrSendToBank(item);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Create)
              .WithProperty(Subject)
              .WithProperty(itemsToGive)
              .LogInformation(
                  "{@AislingName} has received {@Armor}, {@Helm}, {@Weapon} from mastering",
                  source.Name,
                  armor.DisplayName,
                  helm.DisplayName,
                  weapon.DisplayName);
    }

    private void AwardWizardItemsAndAbilities(Aisling source)
    {
        var armor = source.Gender == Gender.Male ? ItemFactory.Create("wizardMasterMantle") : ItemFactory.Create("wizardMasterdress");
        var helm = source.Gender == Gender.Male ? ItemFactory.Create("gnostichat") : ItemFactory.Create("gnosticumber");
        var weapon = ItemFactory.Create("magusorb");
        var fasspiorad = SpellFactory.Create("fasspiorad");

        if (!source.Legend.TryGetValue("dedicated", out _))
        {
            var sightoffrailty = SpellFactory.Create("sightoffrailty");
            source.SpellBook.TryAddToNextSlot(sightoffrailty);
        }

        source.Trackers.Flags.AddFlag(WizardElement.Earth);
        source.Trackers.Flags.AddFlag(WizardElement.Water);
        source.Trackers.Flags.AddFlag(WizardElement.Fire);
        source.Trackers.Flags.AddFlag(WizardElement.Wind);
        source.SpellBook.TryAddToNextSlot(fasspiorad);

        var itemsToGive = new[]
        {
            armor,
            helm,
            weapon
        };

        foreach (var item in itemsToGive)
            source.GiveItemOrSendToBank(item);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Create)
              .WithProperty(Subject)
              .WithProperty(itemsToGive)
              .LogInformation(
                  "{@AislingName} has received {@Armor}, {@Helm}, {@Weapon} from mastering",
                  source.Name,
                  armor.DisplayName,
                  helm.DisplayName,
                  weapon.DisplayName);
    }

    private void NotifyAllAislingsOfNewMaster(Aisling source, BaseClass userBaseClass)
    {
        foreach (var aisling in ClientRegistry)
            aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"{source.Name} now wields the mantle of Master {userBaseClass}");
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "miraelisgod_mastering":
            {
                if (source.UserStatSheet.Master)
                    Subject.Reply(source, "You already wear the Mantle of the Master.");

                if (!source.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
                    Subject.Reply(
                        source,
                        "Ah, you aspire to wield the ancient powers of mastery. Yet, the path to such power is not freely given. It is a journey intertwined with the very essence of a main story that shapes your fate. Until you have walked the full length of this path, the arcane secrets of mastery shall remain veiled from your sight. Return to me only when you have faced the core trials, and I shall reveal the way to harness the true might that awaits you.");

                break;
            }

            case "miraelisgod_checkhealth":
            {
                // Retrieve the base class of the source
                var baseClass = source.UserStatSheet.BaseClass;

                // Check if the base class exists in the dictionary
                if (MasteringRequirements.TryGetValue(baseClass, out var requirements))
                {
                    // Get the required health and mana
                    var requiredHealth = requirements.requiredHealth;

                    // Compare with source's current health and mana
                    var hasEnoughHealth = source.UserStatSheet.MaximumHp >= requiredHealth;

                    if (!hasEnoughHealth)
                        Subject.Reply(
                            source,
                            "It seems your health is not yet at the level required for a master. Endurance and vitality are essential for the challenges you will face. Seek out ways to fortify your health and return when you are stronger.");
                }

                break;
            }
            case "miraelisgod_checkmana":
            {
                // Retrieve the base class of the source
                var baseClass = source.UserStatSheet.BaseClass;

                // Check if the base class exists in the dictionary
                if (MasteringRequirements.TryGetValue(baseClass, out var requirements))
                {
                    // Get the required health and mana
                    var requiredMana = requirements.requiredMana;

                    // Compare with source's current health and mana
                    var hasEnoughMana = source.UserStatSheet.MaximumMp >= requiredMana;

                    if (!hasEnoughMana)
                        Subject.Reply(
                            source,
                            "Your mana reserves are not yet sufficient to meet the demands of a master. Mastery over the arcane is a journey of constant learning and growth. Enhance your mana through study and practice, and return when you are ready.");
                }

                break;
            }

            case "miraelisgod_checkskillsspells":
            {
                // Retrieve the base class of the source
                var baseClass = source.UserStatSheet.BaseClass;

                // Check if the base class exists in the dictionary
                if (MasteringSkillSpellRequirements.TryGetValue(baseClass, out var requirements))
                {
                    // Get the required skill/spell
                    var requiredSkillOne = requirements.skillOne;
                    var requiredSkillTwo = requirements.skillTwo;

                    switch (baseClass)
                    {
                        // Compare with source's current health and mana
                        case BaseClass.Warrior
                            when !source.SkillBook.Contains(requiredSkillOne) && !source.SkillBook.Contains(requiredSkillTwo):
                            Subject.Reply(
                                source,
                                "As a Warrior, you must demonstrate unparalleled prowess in combat. Currently, your skills do not meet the standards of a master. Train rigorously in the art of war and return when your abilities are honed to perfection.");

                            return;
                        case BaseClass.Monk
                            when !source.SkillBook.Contains(requiredSkillOne) && !source.SkillBook.Contains(requiredSkillTwo):
                            Subject.Reply(
                                source,
                                "As a Monk, your path to mastery lies in perfecting harmony of mind, body, and spirit. It appears you have yet to master all the necessary Monk arts. Continue your training and meditation, and return when you have achieved greater mastery.");

                            return;
                        case BaseClass.Rogue
                            when !source.SkillBook.Contains(requiredSkillOne) && !source.SkillBook.Contains(requiredSkillTwo):
                            Subject.Reply(
                                source,
                                "The path of a Rogue is one of stealth and cunning. It seems you have yet to master the necessary arts of subterfuge and agility. Sharpen your skills in the shadows and return when you are truly adept.");

                            return;
                        case BaseClass.Priest
                            when !source.SpellBook.Contains(requiredSkillOne) && !source.SpellBook.Contains(requiredSkillTwo):
                            Subject.Reply(
                                source,
                                "A Priest's mastery is judged by their devotion and their command over healing and protective spells. You are not yet ready. Deepen your faith and knowledge of the sacred arts, then return to me.");

                            return;
                        case BaseClass.Wizard
                            when !source.SpellBook.Contains(requiredSkillOne) && !source.SpellBook.Contains(requiredSkillTwo):
                            Subject.Reply(
                                source,
                                "The essence of a Wizard lies in their mastery of the arcane. It appears you have not yet mastered the necessary spells and arcane knowledge. Dedicate yourself further to the study of magic and return when you have achieved true mastery.");

                            return;

                        case BaseClass.Peasant:
                            Subject.Reply(
                                source,
                                "You are but a filthy peasant. Peasants cannot master their class because they do not have one.");

                            break;

                        case BaseClass.Diacht:
                            Subject.Reply(source, "You shouldn't be here.");

                            break;
                    }
                }

                break;
            }

            case "miraelisgod_checkitems":
            {
                // Retrieve the base class of the source
                var baseClass = source.UserStatSheet.BaseClass;

                // Check if the base class exists in the dictionary
                if (MasteringItemRequirements.TryGetValue(baseClass, out var requirements))
                {
                    // Get the required skill/spell
                    var requiredItemOne = requirements.itemOne;
                    var requiredItemTwo = requirements.itemTwo;

                    switch (baseClass)
                    {
                        // Compare with source's current health and mana
                        case BaseClass.Warrior when !source.Inventory.ContainsByTemplateKey(requiredItemOne)
                                                    || !source.Inventory.ContainsByTemplateKey(requiredItemTwo):
                            Subject.Reply(
                                source,
                                "A true Warrior's might is mirrored in their arms. The Broken Jackal Sword, a symbol of resilience, and the Jackal Hilt, representing unyielding grip, are what you must seek. These items will complete your warrior's arsenal.");

                            return;
                        case BaseClass.Monk when !source.Inventory.ContainsByTemplateKey(requiredItemOne)
                                                 || !source.Inventory.Contains(requiredItemTwo):
                            Subject.Reply(
                                source,
                                "A Monk's mastery is symbolized by items that resonate with the earth and alchemy. Seek the Goldsand that shimmers with inner peace, and the Cauldron that blends the elements in harmony. Return with these, and your path to mastery shall be clear.");

                            return;
                        case BaseClass.Rogue when !source.Inventory.ContainsByTemplateKey(requiredItemOne)
                                                  || !source.Inventory.ContainsByTemplateKey(requiredItemTwo):
                            Subject.Reply(
                                source,
                                "The art of a Rogue is in the details. Seek the Copper File, perfect for crafting the tools of subterfuge, and the Smith Hammer, symbolic of the strength behind every silent move. Return with these, and your mastery shall be undeniable.");

                            return;
                        case BaseClass.Priest when !source.Inventory.ContainsByTemplateKey(requiredItemOne)
                                                   || !source.Inventory.ContainsByTemplateKey(requiredItemTwo):
                            Subject.Reply(
                                source,
                                "As a Priest, you must possess items that are imbued with divine essence. Seek the Holy Ink, a vessel of sacred blessings, and the Holy Scroll, a carrier of celestial prayers. These items will elevate your spiritual communion.");

                            return;
                        case BaseClass.Wizard when !source.Inventory.ContainsByTemplateKey(requiredItemOne)
                                                   || !source.Inventory.ContainsByTemplateKey(requiredItemTwo):
                            Subject.Reply(
                                source,
                                "For a Wizard, the key to mastery lies in the tools of arcane knowledge. Seek the Magic Ink, infused with the essence of spells, and the Magic Scroll, a repository of mystical lore. With these, your path to wizardry mastery shall be complete.");

                            return;

                        case BaseClass.Peasant:
                            Subject.Reply(
                                source,
                                "You are but a filthy peasant. Peasants cannot master their class because they do not have one.");

                            break;

                        case BaseClass.Diacht:
                            Subject.Reply(source, "You shouldn't be here.");

                            break;
                    }
                }

                break;
            }

            case "miraelisgod_becomemaster":
            {
                if (source.UserStatSheet.TotalExp < 250000000)
                {
                    Subject.Reply(
                        source,
                        "You must have at least 250,000,000 experience to become a Master. Please seek the knowledge and return.");

                    return;
                }

                source.UserStatSheet.SubtractTotalExp(250000000);

                var userBaseClass = source.UserStatSheet.BaseClass;

                if (MasteringItemRequirements.TryGetValue(userBaseClass, out var requirements))
                {
                    source.Inventory.RemoveQuantityByTemplateKey(requirements.itemOne, 1);
                    source.Inventory.RemoveQuantityByTemplateKey(requirements.itemTwo, 1);
                }

                NotifyAllAislingsOfNewMaster(source, userBaseClass);
                source.Titles.Add($"Master {userBaseClass}");
                source.UserStatSheet.SetIsMaster(true);
                AwardClassSpecificItemsAndAbilities(source, userBaseClass);

                switch (userBaseClass)
                {
                    case BaseClass.Warrior:
                        AwardMasterLegendMark(source, BaseClass.Warrior, "warMaster");

                        break;
                    case BaseClass.Rogue:
                        AwardMasterLegendMark(source, BaseClass.Rogue, "rogueMaster");

                        break;
                    case BaseClass.Wizard:
                        AwardMasterLegendMark(source, BaseClass.Wizard, "wizMaster");

                        break;
                    case BaseClass.Priest:
                        AwardMasterLegendMark(source, BaseClass.Priest, "priestMaster");

                        break;
                    case BaseClass.Monk:
                        AwardMasterLegendMark(source, BaseClass.Monk, "monkMaster");

                        break;
                }

                source.SendOrangeBarMessage("You have been awarded a title, weapon, armor and helmet!");
                source.Client.SendAttributes(StatUpdateType.Full);
                source.Trackers.Enums.Set(ClassStatBracket.Master);

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote, Topics.Entities.Quest)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} has become a master aisling", source.Name);

                break;
            }
        }
    }
}