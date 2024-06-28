using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Models.World;

namespace Chaos.Scripting.Components.AbilityComponents
{
    public struct ShowDialogAbilityComponent : IComponent
    {
        /// <inheritdoc />
        public void Execute(ActivationContext context, ComponentVars vars)
        {
            var options = vars.GetOptions<IShowDialogComponentOptions>();

            if (string.IsNullOrEmpty(options.DialogKey))
                return;

            var targetAisling = context.TargetAisling;

            if (targetAisling == null)
                return;

            // Anytime a person reads dialog, it will check these. We can use this to make sure we clean up anything they have that they shouldn't
            if (targetAisling.UserStatSheet.BaseClass is BaseClass.Warrior)
                if (targetAisling.SkillBook.ContainsByTemplateKey("beagsuain") && targetAisling.SkillBook.ContainsByTemplateKey("groundstomp"))
                    targetAisling.SkillBook.RemoveByTemplateKey("beagsuain");

            if (targetAisling.UserStatSheet.BaseClass is BaseClass.Monk)
                if (targetAisling.Trackers.Enums.TryGetValue(out MonkElementForm form))
                {
                    CleanUpSkillsAndSpells(targetAisling, form);
                }

            targetAisling.DialogHistory.Clear();
            var dialog = options.DialogFactory.Create(options.DialogKey, options.DialogSource);
            dialog.Display(targetAisling);
        }

        private void CleanUpSkillsAndSpells(Aisling targetAisling, MonkElementForm form)
        {
            List<string> allowedSkills = [];
            List<string> allowedSpells = [];

            switch (form)
            {
                case MonkElementForm.Water:
                    allowedSkills = ["waterpunch", "tsunamikick"];
                    allowedSpells = ["miststance"];
                    break;
                case MonkElementForm.Earth:
                    allowedSkills = ["earthpunch", "seismickick"];
                    allowedSpells = ["earthenstance"];
                    break;
                case MonkElementForm.Air:
                    allowedSkills = ["airpunch", "tempestkick"];
                    allowedSpells = ["thunderstance"];
                    break;
                case MonkElementForm.Fire:
                    allowedSkills = ["firepunch", "dracotailkick"];
                    allowedSpells = ["smokestance"];
                    break;
            }
            

            // Remove any skills or spells not matching the allowed list
            foreach (var skill in targetAisling.SkillBook)
            {
                if (!allowedSkills.Contains(skill.Template.TemplateKey))
                {
                    targetAisling.SkillBook.RemoveByTemplateKey(skill.Template.TemplateKey);
                }
            }

            foreach (var spell in targetAisling.SpellBook)
            {
                if (!allowedSpells.Contains(spell.Template.TemplateKey))
                {
                    targetAisling.SpellBook.RemoveByTemplateKey(spell.Template.TemplateKey);
                }
            }
        }

        public interface IShowDialogComponentOptions
        {
            IDialogFactory DialogFactory { get; init; }
            string? DialogKey { get; init; }
            IDialogSourceEntity DialogSource { get; init; }
        }
    }
}
