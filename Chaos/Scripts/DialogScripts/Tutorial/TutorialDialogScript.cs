using System.Diagnostics.Eventing.Reader;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.DialogScripts;

public class TutorialDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

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
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "leia_initial":
                if (!source.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "leia_1",
                        OptionText = "I'll stay, what has changed? What happen?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if (source.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor)
                    && !source.Flags.HasFlag(TutorialQuestFlag.GaveAssailAndSpell))
                {
                    source.SendOrangeBarMessage("Equip your stick and armor, then say hello to get Leia's attention.");

                    return;
                }

                break;
            case "leia_1":
                if (!source.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor))
                {
                    source.Flags.AddFlag(TutorialQuestFlag.GaveStickAndArmor);

                    var stick = ItemFactory.Create("stick");
                    var armor = source.Gender == Gender.Female ? ItemFactory.Create("blouse") : ItemFactory.Create("shirt");

                    source.TryGiveItems(stick, armor);
                }

                break;
            case "leia_2":
                if (source.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor)
                    && !source.Flags.HasFlag(TutorialQuestFlag.GaveAssailAndSpell))
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
                source.Flags.AddFlag(TutorialQuestFlag.GaveAssailAndSpell);

                break;

            case "cain_initial":
                if (source.Flags.HasFlag(TutorialQuestFlag.GaveAssailAndSpell) && !source.Flags.HasFlag(TutorialQuestFlag.StartedFloppy))
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
                        Subject.Options.Insert(0, option1);

                    return;
                }
                if (source.Flags.HasFlag(TutorialQuestFlag.StartedFloppy) && (!source.Flags.HasFlag(TutorialQuestFlag.CompletedFloppy)))
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
                        Subject.Options.Insert(0, option1);

                    return;
                }
                
                if (source.Flags.HasFlag(TutorialQuestFlag.CompletedFloppy) && (!source.Flags.HasFlag(TutorialQuestFlag.GotEquipment)))
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

                if (source.Flags.HasFlag(TutorialQuestFlag.GotEquipment) && (!source.Flags.HasFlag(TutorialQuestFlag.GiantFloppy)))
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
                
                if (source.Flags.HasFlag(TutorialQuestFlag.GiantFloppy))
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
                if (source.Flags.HasFlag(TutorialQuestFlag.GaveAssailAndSpell) && !source.Flags.HasFlag(TutorialQuestFlag.StartedFloppy))
                {
                    source.Flags.AddFlag(TutorialQuestFlag.StartedFloppy);
                }

                break;
            
            
            case "cain_yes2":
                if (source.Flags.HasFlag(TutorialQuestFlag.StartedFloppy) && !source.Flags.HasFlag(TutorialQuestFlag.CompletedFloppy))
                {

                    if (!source.Inventory.HasCount("carrot", 3))
                    {
                        source.SendOrangeBarMessage("Farmer looks disappointed, you don't have enough carrots.");
                        Subject.Close(source);

                        return;
                    }
                    source.Inventory.RemoveQuantity("carrot", 3);
                    source.GiveExp(1000);
                    source.TryGiveGold(1000);
                    source.Flags.AddFlag(TutorialQuestFlag.CompletedFloppy);

                    return;
                }
                break;
            
            case "tutorialequipment":
                if (source.Flags.HasFlag(TutorialQuestFlag.CompletedFloppy) && !source.Flags.HasFlag(TutorialQuestFlag.GotEquipment))
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
                    source.Flags.AddFlag(TutorialQuestFlag.GotEquipment);
                }

                break;
            case "tutorialgiantfloppy":
                if (source.Flags.HasFlag(TutorialQuestFlag.GotEquipment) && !source.Flags.HasFlag(TutorialQuestFlag.GiantFloppy))
                {
                    source.Flags.AddFlag(TutorialQuestFlag.GiantFloppy);
                }
                break;
        }
    }
}