using System.Diagnostics.Eventing.Reader;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.DialogScripts;

public class TutorialDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private IExperienceDistributionScript ExperienceDistributionScript{ get; set; }
    /// <inheritdoc />
    public TutorialDialogScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        
        var hasStage = source.Enums.TryGetValue(out TutorialQuestStage stage);
        
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
                        OptionText ="Can you repeat that?"
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
                    
                    return;
                }

                break;
                
            case "leia_1":
                if (!hasStage)
                {
                    source.Enums.Set(TutorialQuestStage.GaveStickAndArmor);

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

                var assail = SkillFactory.Create("assail");
                var sradtut = SpellFactory.Create("sradtut");
                source.SkillBook.TryAddToNextSlot(assail);
                source.SpellBook.TryAddToNextSlot(sradtut);
                source.Enums.Set(TutorialQuestStage.GaveAssailAndSpell);

                break;
            
            case "leiaend":
                if (stage == TutorialQuestStage.GaveAssailAndSpell)
                {
                    ExperienceDistributionScript.GiveExp(source, 250);
                    source.TryGiveGold(1000);
                    source.Enums.Set(TutorialQuestStage.LearnedWorld);

                    return;
                }

                break;
            case "cain_initial":
                if (stage == TutorialQuestStage.GotEquipment)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cain_yes",
                        OptionText = "I'm on it!",

                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "cain_no",
                        OptionText = "Not right now.",

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
                        OptionText = "Here ya go",

                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "cain_no2",
                        OptionText = "No, not yet, those floppies hurt..",

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
                        OptionText = "Buying Equipment",
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
                        OptionText = "Giant Floppy",
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
                        OptionText = "Giant Floppy",
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;

            case "cain_yes":
                if (stage == TutorialQuestStage.GotEquipment)
                {
                    source.Enums.Set(TutorialQuestStage.StartedFloppy);
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
                    source.Enums.Set(TutorialQuestStage.CompletedFloppy);

                    return;
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
                        OptionText = "I got rings and boots from Abel",
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                    source.Enums.Set(TutorialQuestStage.GotEquipment);
                    return;
                }

                break;
            case "tutorialgiantfloppy":
                if (stage == TutorialQuestStage.CompletedFloppy)
                {
                    source.Enums.Set(TutorialQuestStage.GiantFloppy);
                }
                break;
        }
    }
}