using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

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

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if (stage != TutorialQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "leia_repeat",
                        OptionText = "Can you repeat that?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if (stage == TutorialQuestStage.GaveStickAndArmor)
                {
                    source.SendOrangeBarMessage("Equip your stick and armor, then say hello to get Leia's attention.");

                    return;
                }

                if (stage == TutorialQuestStage.GaveAssailAndSpell)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "learnworld",
                        OptionText = "What else have I missed?"
                    };

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "leia_1":
                if (!hasStage)
                {
                    source.Trackers.Enums.Set(TutorialQuestStage.GaveStickAndArmor);

                    var stick = ItemFactory.Create("stick");
                    var armor = source.Gender == Gender.Female ? ItemFactory.Create("blouse") : ItemFactory.Create("shirt");

                    source.TryGiveItems(stick, armor);
                }

                break;
            case "leia_2":
                if (stage == TutorialQuestStage.GaveStickAndArmor)
                {
                    var weapon = source.Equipment[EquipmentSlot.Weapon];
                    var armor = source.Equipment[EquipmentSlot.Armor];

                    if ((weapon == null)
                        || (armor == null)
                        || !weapon.DisplayName.EqualsI("stick")
                        || (!armor.DisplayName.EqualsI("shirt") && !armor.DisplayName.EqualsI("blouse")))
                    {
                        source.SendOrangeBarMessage("Equip armor and weapon then say Hello.");
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
                    ExperienceDistributionScript.GiveExp(source, 250);
                    source.TryGiveGold(1000);
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

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
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

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
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

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
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
                    ExperienceDistributionScript.GiveExp(source, 500);
                    source.TryGiveGold(1000);
                    source.Trackers.Enums.Set(TutorialQuestStage.CompletedFloppy);

                    var option = new DialogOption
                    {
                        DialogKey = "tutorialgiantfloppy",
                        OptionText = "Giant Floppy"
                    };

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    source.Trackers.Enums.Set(TutorialQuestStage.GiantFloppy);
                }

                break;

            case "leia_exit2":
            {
                SpellFactory.Create("sradtut");
                var stick = ItemFactory.Create("stick");
                var armor = source.Gender == Gender.Female ? ItemFactory.Create("blouse") : ItemFactory.Create("shirt");
                var ring = ItemFactory.Create("smallrubyring");
                var boots = ItemFactory.Create("boots");
                assail = SkillFactory.Create("assail");
                MapInstance? mapInstance;
                Point point;

                if (hasStage)
                {
                    if (stage == TutorialQuestStage.GaveStickAndArmor)
                    {
                        source.TryGiveGold(1300);
                        source.TryGiveItems(ring, ring, boots);
                        assail = SkillFactory.Create("assail");
                        source.SkillBook.TryAddToNextSlot(assail);
                        ExperienceDistributionScript.GiveExp(source, 1750);

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Completed Tutorial",
                                "CompletedTutorial",
                                MarkIcon.Heart,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage == TutorialQuestStage.GaveAssailAndSpell)
                    {
                        source.TryGiveGold(1300);
                        source.TryGiveItems(ring, ring, boots);
                        source.SkillBook.TryAddToNextSlot(assail);
                        ExperienceDistributionScript.GiveExp(source, 1750);
                        source.SpellBook.Remove("srad tut");

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Completed Tutorial",
                                "CompletedTutorial",
                                MarkIcon.Heart,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage == TutorialQuestStage.LearnedWorld)
                    {
                        source.TryGiveGold(1300);
                        source.TryGiveItems(ring, ring, boots);
                        ExperienceDistributionScript.GiveExp(source, 1500);
                        source.SpellBook.Remove("srad tut");

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Completed Tutorial",
                                "CompletedTutorial",
                                MarkIcon.Heart,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage is TutorialQuestStage.GotEquipment or TutorialQuestStage.StartedFloppy)
                    {
                        source.TryGiveGold(1000);
                        ExperienceDistributionScript.GiveExp(source, 1500);
                        source.SpellBook.Remove("srad tut");

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Completed Tutorial",
                                "CompletedTutorial",
                                MarkIcon.Heart,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }

                    if (stage is TutorialQuestStage.CompletedFloppy or TutorialQuestStage.GiantFloppy)

                    {
                        ExperienceDistributionScript.GiveExp(source, 1000);
                        source.SpellBook.Remove("srad tut");

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Completed Tutorial",
                                "CompletedTutorial",
                                MarkIcon.Heart,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                        point = new Point(5, 8);
                        mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(mapInstance, point);

                        return;
                    }
                }

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Completed Tutorial",
                        "CompletedTutorial",
                        MarkIcon.Heart,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.TryGiveItems(
                    stick,
                    armor,
                    ring,
                    ring,
                    boots);

                assail = SkillFactory.Create("assail");
                source.SkillBook.TryAddToNextSlot(assail);
                ExperienceDistributionScript.GiveExp(source, 1750);
                source.SpellBook.Remove("srad tut");
                source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);
                point = new Point(5, 8);
                mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                source.TraverseMap(mapInstance, point);

                break;
            }
        }
    }
}