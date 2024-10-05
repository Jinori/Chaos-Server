using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Tutorial;

public class TutorialDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public TutorialDialogScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out TutorialQuestStage stage);

        Skill? assail;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "leia_initial":
                if (!hasStage)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "leia_1",
                        OptionText = "I'll stay, what has changed? Where am I?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (stage != TutorialQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "leia_repeat",
                        OptionText = "Can you repeat that?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (stage == TutorialQuestStage.GaveArmor)
                {
                    source.SendOrangeBarMessage("Equip your armor, then say hello to get Leia's attention.");

                    return;
                }

                if (stage == TutorialQuestStage.GaveAssailAndSpell)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "learnworld",
                        OptionText = "What else have I missed?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == TutorialQuestStage.LearnedWorld)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "talktocain",
                        OptionText = "I'll go talk to Cain now."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "leia_1":
                if (!hasStage)
                {
                    source.Trackers.Enums.Set(TutorialQuestStage.GaveArmor);
                    source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                    
                    var armor = source.Gender == Gender.Female ? ItemFactory.Create("blouse") : ItemFactory.Create("shirt");

                    source.GiveItemOrSendToBank(armor);
                }

                break;
            case "leia_2":
                if (stage == TutorialQuestStage.GaveArmor)
                {
                    var armor = source.Equipment[EquipmentSlot.Armor];

                    if ((armor == null)
                        || (!armor.DisplayName.EqualsI("shirt") && !armor.DisplayName.EqualsI("blouse")))
                    {
                        source.SendOrangeBarMessage("Equip armor then say Hello.");
                        Subject.Close(source);

                        return;
                    }
                }

                assail = SkillFactory.Create("assail");
                var sradtut = SpellFactory.Create("sradtut");
                source.SkillBook.TryAddToNextSlot(assail);
                source.SpellBook.TryAddToNextSlot(sradtut);
                source.Trackers.Enums.Set(TutorialQuestStage.GaveAssailAndSpell);

                break;

            case "leiaend":
                if (stage == TutorialQuestStage.GaveAssailAndSpell)
                {
                    ExperienceDistributionScript.GiveExp(source, 400);
                    source.TryGiveGold(1000);
                    source.SendOrangeBarMessage("You gained 1000 Gold and 400 exp!");
                    source.Trackers.Enums.Set(TutorialQuestStage.LearnedWorld);
                }

                break;
            case "cain_initial":
                if (stage == TutorialQuestStage.GotEquipment)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cain_yes",
                        OptionText = "I'm on it!"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "cain_no",
                        OptionText = "Not right now."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == TutorialQuestStage.StartedFloppy)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cain_yes2",
                        OptionText = "Here ya go"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "cain_no2",
                        OptionText = "No, not yet, those floppies hurt.."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == TutorialQuestStage.LearnedWorld)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "tutorialequipment",
                        OptionText = "Buying Equipment"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == TutorialQuestStage.CompletedFloppy)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "tutorialgiantfloppy",
                        OptionText = "Giant Floppy"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "tutorialgiantfloppy2",
                        OptionText = "Giant Floppy"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "cain_yes":
                if (stage == TutorialQuestStage.GotEquipment)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cain_yes2",
                        OptionText = "Here ya go."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    source.Trackers.Enums.Set(TutorialQuestStage.StartedFloppy);
                }

                break;

            case "cain_yes2":
                if (stage == TutorialQuestStage.StartedFloppy)
                {
                    if (!source.Inventory.HasCount("carrot", 3))
                    {
                        source.SendOrangeBarMessage("Farmer looks disappointed, you don't have enough carrots.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("carrot", 3);
                    ExperienceDistributionScript.GiveExp(source, 1000);
                    source.TryGiveGold(1000);
                    source.Trackers.Enums.Set(TutorialQuestStage.CompletedFloppy);

                    var option = new DialogOption
                    {
                        DialogKey = "tutorialgiantfloppy",
                        OptionText = "Giant Floppy"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "tutorialequipment":
                if (stage == TutorialQuestStage.LearnedWorld)
                {
                    var ring = source.Equipment[EquipmentSlot.RightRing];
                    var ring2 = source.Equipment[EquipmentSlot.LeftRing];
                    var boots = source.Equipment[EquipmentSlot.Boots];

                    if ((ring == null)
                        || (boots == null)
                        || (ring2 == null))
                    {
                        source.SendOrangeBarMessage("Buy rings and boots from Abel then equip them.");

                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "gotequipment",
                        OptionText = "I got rings and boots from Abel"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    source.Trackers.Enums.Set(TutorialQuestStage.GotEquipment);
                }

                break;
            case "tutorialgiantfloppy":
                if (stage == TutorialQuestStage.CompletedFloppy)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "tutorialgiantfloppy",
                        OptionText = "Giant Floppy"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    source.Trackers.Enums.Set(TutorialQuestStage.GiantFloppy);
                }

                break;

            case "leia_exit2":
            {
                SpellFactory.Create("sradtut");
                var armor = source.Gender == Gender.Female ? ItemFactory.Create("blouse") : ItemFactory.Create("shirt");
                var ring1 = ItemFactory.Create("smallrubyring");
                var ring2 = ItemFactory.Create("smallrubyring");
                var boots = ItemFactory.Create("boots");
                assail = SkillFactory.Create("assail");
                MapInstance? mapInstance;
                Point point;

                if (hasStage)
                {
                    if (stage == TutorialQuestStage.GaveArmor)
                    {
                        source.GiveItemOrSendToBank(ring1);
                        source.GiveItemOrSendToBank(ring2);
                        source.GiveItemOrSendToBank(boots);
                        assail = SkillFactory.Create("assail");
                        source.SkillBook.TryAddToNextSlot(assail);
                        source.TryGiveGold(2000);
                        ExperienceDistributionScript.GiveExp(source, 2000);

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage == TutorialQuestStage.GaveAssailAndSpell)
                    {
                        source.GiveItemOrSendToBank(ring1);
                        source.GiveItemOrSendToBank(ring2); 
                        source.GiveItemOrSendToBank(boots);
                        source.SkillBook.TryAddToNextSlot(assail);
                        source.TryGiveGold(2000);
                        ExperienceDistributionScript.GiveExp(source, 2000);
                        source.SpellBook.Remove("srad tut");
                        
                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage == TutorialQuestStage.LearnedWorld)
                    {
                        source.GiveItemOrSendToBank(ring1);
                        source.GiveItemOrSendToBank(ring2);
                        source.GiveItemOrSendToBank(boots);
                        source.TryGiveGold(1000);
                        ExperienceDistributionScript.GiveExp(source, 1400);
                        source.SpellBook.Remove("srad tut");

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage is TutorialQuestStage.GotEquipment or TutorialQuestStage.StartedFloppy)
                    {
                        source.TryGiveGold(1000);
                        ExperienceDistributionScript.GiveExp(source, 1400);
                        source.SpellBook.Remove("srad tut");

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage is TutorialQuestStage.CompletedFloppy or TutorialQuestStage.GiantFloppy)

                    {
                        ExperienceDistributionScript.GiveExp(source, 1000);
                        source.SpellBook.Remove("srad tut");

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }
                }

                source.GiveItemOrSendToBank(armor);
                source.GiveItemOrSendToBank(ring1);
                source.GiveItemOrSendToBank(ring2);
                source.GiveItemOrSendToBank(boots);

                assail = SkillFactory.Create("assail");
                source.SkillBook.TryAddToNextSlot(assail);
                source.TryGiveGold(2000);
                ExperienceDistributionScript.GiveExp(source, 2000);
                source.SpellBook.Remove("srad tut");
                source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                source.Trackers.Enums.Set(ClassStatBracket.PreMaster);
                point = new Point(5, 8);
                mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                source.TraverseMap(mapInstance, point);

                break;
            }
        }
    }
}