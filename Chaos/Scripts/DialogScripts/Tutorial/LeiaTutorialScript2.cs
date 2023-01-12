using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Tutorial
{
    public class LeiaTutorialScript2 : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISkillFactory SkillFactory;
        private readonly ISpellFactory SpellFactory;

        public LeiaTutorialScript2(Dialog subject, IItemFactory itemFactory, ISkillFactory skillFactory, ISpellFactory spellFactory) : base(subject)
        {
            ItemFactory = itemFactory;
            SkillFactory = skillFactory;
            SpellFactory = spellFactory;
        }
        public override void OnDisplayed(Aisling source)
        {
            if (source.Flags.HasFlag(TutorialFlag.LeiaTutorialFlag1)
                && source.Equipment.Any(Item => Item.Template.TemplateKey.EqualsI("Stick"))
                && (source.Equipment.Any(Item => Item.Template.TemplateKey.EqualsI("shirt"))
                || source.Equipment.Any(Item => Item.Template.TemplateKey.EqualsI("Blouse"))))
                
            {
                source.Flags.RemoveFlag(TutorialFlag.LeiaTutorialFlag1);
                source.Flags.AddFlag(TutorialFlag.LeiaTutorialFlag2);
                Subject.Text = "Looking good! Now that you have some equipment. Let's learn how to attack and cast spells.\n To use Assail, hit spacebar or hotkey 's' and click the skill, or use the number associated.\nTo cast a spell, there's a few ways to do this. You can drag the spell over a target, click the spell and click the target, or hit the number associated then click the target.\nIt's all up to you. Go outside and help Cain, it sounds like he's struggling with those floppies again.";
                Subject.Type = MenuOrDialogType.Normal;
                var skill = SkillFactory.Create("assail");
                if (!source.SkillBook.Contains(skill))
                {
                    source.SkillBook.TryAddToNextSlot(skill);
                }
                var spell = SpellFactory.Create("srad tut");
                if (!source.SpellBook.Contains(spell))
                {
                    source.SpellBook.TryAddToNextSlot(spell);
                }
            }
            else
            {
                Subject.Text = "Please equip the stick and armor then talk to me again.";
                Subject.Type = MenuOrDialogType.Normal;
            }
        }
    }
}
