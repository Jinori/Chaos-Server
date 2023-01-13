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
    public TutorialDialogScript(Dialog subject, IItemFactory itemFactory, ISkillFactory skillFactory,
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
        switch (Subject.Template.TemplateKey)
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

                if (source.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor) && !source.Flags.HasFlag(TutorialQuestFlag.GaveAssail))
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
                if (source.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor) && !source.Flags.HasFlag(TutorialQuestFlag.GaveAssail))
                {
                    var weapon = source.Equipment[EquipmentSlot.Weapon];
                    var armor = source.Equipment[EquipmentSlot.Armor];

                    if ((weapon == null)
                        || (armor == null)
                        || !weapon.DisplayName.EqualsI("stick")
                        || (!armor.DisplayName.EqualsI("shirt") && !armor.DisplayName.EqualsI("blouse")))
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage("Equip your stick and armor, then say hello again to get Leia's attention.");

                        return;
                    }

                    source.Flags.AddFlag(TutorialQuestFlag.GaveAssail);
                }

                var assail = SkillFactory.Create("assail");
                var sradtut = SpellFactory.Create("sradtut");
                source.SkillBook.TryAddToNextSlot(assail);
                source.SpellBook.TryAddToNextSlot(sradtut);
                source.Flags.AddFlag(TutorialQuestFlag.GaveAssail);

                break;
        }
    }
}