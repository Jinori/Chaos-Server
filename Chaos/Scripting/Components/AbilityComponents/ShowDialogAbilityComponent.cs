using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Models.World;
using System.Collections.Generic;

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

            CleanUpSkillsAndSpells(targetAisling);

            targetAisling.DialogHistory.Clear();
            var dialog = options.DialogFactory.Create(options.DialogKey, options.DialogSource);
            dialog.Display(targetAisling);
        }

        private void CleanUpSkillsAndSpells(Aisling targetAisling)
        {
            if (targetAisling.UserStatSheet.BaseClass is BaseClass.Warrior)
            {
                if (targetAisling.SkillBook.ContainsByTemplateKey("beagsuain") && targetAisling.SkillBook.ContainsByTemplateKey("groundstomp"))
                    targetAisling.SkillBook.RemoveByTemplateKey("beagsuain");
                return;
            }

            if (targetAisling.UserStatSheet.BaseClass is BaseClass.Monk && targetAisling.Trackers.Enums.TryGetValue(out MonkElementForm form))
            {
                var elementSkillsAndSpells = new Dictionary<MonkElementForm, (List<string> Skills, List<string> Spells)>
                {
                    { MonkElementForm.Water, (["waterpunch", "tsunamikick"], ["miststance"]) },
                    { MonkElementForm.Earth, (["earthpunch", "seismickick"], ["earthenstance"]) },
                    { MonkElementForm.Air, (["airpunch", "tempestkick"], ["thunderstance"]) },
                    { MonkElementForm.Fire, (["firepunch", "dracotailkick"], ["smokestance"]) }
                };

                foreach (var element in elementSkillsAndSpells)
                {
                    if (element.Key != form)
                    {
                        // Remove skills of other elements
                        foreach (var skill in element.Value.Skills)
                        {
                            if (targetAisling.SkillBook.ContainsByTemplateKey(skill))
                            {
                                targetAisling.SkillBook.RemoveByTemplateKey(skill);
                            }
                        }
                        // Remove spells of other elements
                        foreach (var spell in element.Value.Spells)
                        {
                            if (targetAisling.SpellBook.ContainsByTemplateKey(spell))
                            {
                                targetAisling.SpellBook.RemoveByTemplateKey(spell);
                            }
                        }
                    }
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
