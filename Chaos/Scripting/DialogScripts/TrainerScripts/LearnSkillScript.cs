using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.TrainerScripts
{
    public class LearnSkillScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ILogger<LearnSkillScript> Logger;
        private readonly ISkillFactory SkillFactory;
        private readonly ISkillTeacherSource SkillTeacherSource;
        
        private bool IsPureMaster(Aisling source)
        {
            return source.Legend.ContainsKey("dedicated");
        }

        private readonly Dictionary<string, List<string>> SkillUpgrades = new()
        {
            //Warrior
            { "scathe", new List<string> { "cleave" } },
            { "strike", new List<string> { "clobber", "flank", "wallop", "pulverize", "thrash" } },
            { "clobber", new List<string> { "flank", "wallop", "pulverize", "thrash" } },
            { "wallop", new List<string> { "pulverize", "thrash" } },
            { "slash", new List<string> { "sunder" } },
            { "windblade", new List<string> { "tempestblade" } },
            {"groundstomp", new List<string> { "paralyzeforce"} },
            {"flurry", new List<string> {"madsoul"} },
            {"bullrush", new List<string> {"charge"} },
            //Monk
            { "punch", new List<string> { "doublepunch", "rapidpunch", "triplekick" } },
            { "doublepunch", new List<string> { "rapidpunch", "triplekick" }},
            { "kick", new List<string> { "roundhousekick" } },
            { "highkick", new List<string> { "mantiskick" } },
            { "eaglestrike", new List<string> { "phoenixstrike", "dragonstrike" } },
            { "phoenixstrike", new List<string> { "dragonstrike" } },
            //Rogue
            { "assault", new List<string> { "throwsurigum", "blitz", "barrage" } },
            { "stab", new List<string> { "gut" } },
            { "pierce", new List<string> { "skewer" } },
        };

        private readonly Dictionary<string, List<string>> PureAbilities = new()
        {
            { "annihilate", new List<string> { "annihilate" } },
            { "dragonstrike", new List<string> { "dragonstrike" } },
            { "chaosfist", new List<string> { "chaosfist" } },
            { "madsoul", new List<string> { "madsoul" } },
        };

        
        private readonly ISpellFactory SpellFactory;

        public LearnSkillScript(
            Dialog subject,
            IItemFactory itemFactory,
            ISkillFactory skillFactory,
            ISpellFactory spellFactory,
            ILogger<LearnSkillScript> logger)
            : base(subject)
        {
            ItemFactory = itemFactory;
            SkillFactory = skillFactory;
            SpellFactory = spellFactory;
            Logger = logger;
            SkillTeacherSource = (ISkillTeacherSource)Subject.DialogSource;
        }

        public override void OnDisplaying(Aisling source)
        {
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "generic_learnskill_initial":
                    OnDisplayingInitial(source);
                    break;
                case "generic_learnskill_showrequirements":
                    OnDisplayingRequirements(source);
                    break;
                case "generic_learnskill_accepted":
                    OnDisplayingAccepted(source);
                    break;
            }
        }

        private void OnDisplayingAccepted(Aisling source)
        {
            if (!TryFetchArgs<string>(out var skillName)
                || !TryGetSkill(skillName, source, out var skill)
                || source.SkillBook.Contains(skillName))
            {
                Subject.ReplyToUnknownInput(source);
                return;
            }

            if (!ValidateAndTakeRequirements(source, Subject, skill))
                return;

            var skillToLearn = SkillFactory.Create(skill.Template.TemplateKey);

            var learnSkillResult = ComplexActionHelper.LearnSkill(source, skillToLearn);

            switch (learnSkillResult)
            {
                case ComplexActionHelper.LearnSkillResult.Success:
                    Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Skill, Topics.Actions.Learn)
                          .WithProperty(Subject)
                          .WithProperty(Subject.DialogSource)
                          .WithProperty(source)
                          .WithProperty(skill)
                          .LogInformation("Aisling {@AislingName} learned skill {@SkillName}", source.Name, skill.Template.Name);

                    var animation = new Animation
                    {
                        AnimationSpeed = 50,
                        TargetAnimation = 22
                    };

                    source.Animate(animation);

                    if (skillToLearn.Template.TemplateKey.Equals("summonpet", StringComparison.CurrentCultureIgnoreCase) && !source.Inventory.ContainsByTemplateKey("petcollar") && !source.Bank.Contains("Pet Collar"))
                    {
                        var collar = ItemFactory.Create("petcollar");
                        source.GiveItemOrSendToBank(collar);
                    }

                    if (!string.IsNullOrEmpty(skillToLearn.Template.LearningRequirements?.SkillSpellToUpgrade))
                    {
                        var oldSkill = source.SkillBook.FirstOrDefault(
                            s => s.Template.TemplateKey.Equals(
                                skillToLearn.Template.LearningRequirements?.SkillSpellToUpgrade,
                                StringComparison.OrdinalIgnoreCase));

                        if (oldSkill != null)
                        {
                            source.SkillBook.Remove(oldSkill.Template.Name);
                            source.SendOrangeBarMessage($"{oldSkill.Template.Name} has been upgraded to {skillToLearn.Template.Name}.");
                        }
                    }

                    break;
                case ComplexActionHelper.LearnSkillResult.NoRoom:
                    Subject.Reply(
                        source,
                        "Like the spilled contents of an unbalanced cup, some knowledge is best forgotten...",
                        "generic_learnskill_initial");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisplayingInitial(Aisling source)
        {
            Subject.Skills.Clear();

            foreach (var skill in SkillTeacherSource.SkillsToTeach)
            {
                if (!IsSkillLearnable(skill, source))
                    continue;

                Subject.Skills.Add(skill);
            }
        }

        private bool IsSkillLearnable(Skill skill, Aisling source)
        {
            var requiredBaseClass = skill.Template.Class;
            var requiredAdvClass = skill.Template.AdvClass;
            var requiredMaster = skill.Template.RequiresMaster;

            if (requiredBaseClass.HasValue && !source.HasClass(requiredBaseClass.Value))
            {
                return false;
            }

            if (requiredMaster && !source.UserStatSheet.Master)
            {
                return false;
            }

            if (requiredAdvClass.HasValue && (requiredAdvClass.Value != source.UserStatSheet.AdvClass))
            {
                return false;
            }

            if (source.SkillBook.Contains(skill))
            {
                return false;
            }
            
            if (PureAbilities.ContainsKey(skill.Template.TemplateKey.ToLower()) && IsPureMaster(source))
            {
                return false;
            }
            

// Check if the current skill is part of an upgrade chain
            if (SkillUpgrades.Values.Any(upgrades => upgrades.Contains(skill.Template.TemplateKey, StringComparer.OrdinalIgnoreCase)))
            {
                // Identify the base skill key
                var baseSkillKey = SkillUpgrades
                    .FirstOrDefault(kvp => kvp.Value.Contains(skill.Template.TemplateKey, StringComparer.OrdinalIgnoreCase))
                    .Key;

                // Find the index of the current skill in its upgrade chain (case-insensitive)
                var upgradeIndex = SkillUpgrades[baseSkillKey]
                    .FindIndex(u => string.Equals(u, skill.Template.TemplateKey, StringComparison.OrdinalIgnoreCase));

                /*// Debugging information
                Console.WriteLine($"Base Skill Key: {baseSkillKey}");
                Console.WriteLine($"Skill Template Key: {skill.Template.TemplateKey}");
                Console.WriteLine($"Upgrade Index: {upgradeIndex}");
                Console.WriteLine($"Upgrades List: {string.Join(", ", SkillUpgrades[baseSkillKey])}");*/

                switch (upgradeIndex)
                {
                    case -1:
                        //Console.WriteLine($"Skill {skill.Template.TemplateKey} not found in upgrade chain for base skill {baseSkillKey}.");
                        return false;
                    case > 0:
                    {
                        var precedingSkill = SkillUpgrades[baseSkillKey][upgradeIndex - 1];

                        if (source.SkillBook.All(s => !string.Equals(s.Template.TemplateKey, precedingSkill, StringComparison.OrdinalIgnoreCase)))
                        {
                            return false;
                        }

                        break;
                    }
                    default:
                    {
                        if (source.SkillBook.All(s => !string.Equals(s.Template.TemplateKey, baseSkillKey, StringComparison.OrdinalIgnoreCase)))
                        {
                            return false;
                        }

                        break;
                    }
                }
            }


            // Ensure that only the base skill is shown if neither the base skill nor the upgrade is known
            if (SkillUpgrades.TryGetValue(skill.Template.TemplateKey, out var upgrades))
            {
                if (upgrades.Any(
                        upgrade => source.SkillBook.Any(
                            s => string.Equals(s.Template.TemplateKey, upgrade, StringComparison.OrdinalIgnoreCase))))
                {
                    return false;
                }
            }

            return true;
        }
        
        private void OnDisplayingRequirements(Aisling source)
        {
            if (!TryFetchArgs<string>(out var skillName)
                || !TryGetSkill(skillName, source, out var skill)
                || source.SkillBook.Contains(skillName))
            {
                Subject.ReplyToUnknownInput(source);
                return;
            }

            var learningRequirementsStr = skill.Template
                                               .LearningRequirements
                                               ?.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory)
                                               .ToString();

            Subject.InjectTextParameters(skill.Template.Description ?? string.Empty, learningRequirementsStr ?? string.Empty);
        }

        private bool TryGetSkill(string skillName, Aisling source, [MaybeNullWhen(false)] out Skill skill)
        {
            skill = SkillTeacherSource.SkillsToTeach.FirstOrDefault(
                skill => skill.Template.Name.EqualsI(skillName)
                         && source.HasClass(skill.Template.Class!.Value)
                         && (!skill.Template.AdvClass.HasValue || (source.UserStatSheet.AdvClass == skill.Template.AdvClass.Value)));

            return skill != null;
        }

        public bool ValidateAndTakeRequirements(Aisling source, Dialog dialog, Skill skillToLearn)
        {
            var template = skillToLearn.Template;
            var requirements = template.LearningRequirements;

            if (requirements == null)
                return true;

            if (template.Class.HasValue && !source.HasClass(template.Class.Value))
            {
                HandleInvalidLearningAttempt(dialog, source, skillToLearn, "not the correct class (possibly packeting)");
                return false;
            }

            if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
            {
                HandleInvalidLearningAttempt(dialog, source, skillToLearn, "not the correct adv class (possibly packeting)");
                return false;
            }

            if (source.StatSheet.Level < template.Level)
            {
                dialog.Reply(source, "Come back when you are more experienced.", "generic_learnskill_initial");
                return false;
            }

            if (template.RequiresMaster && !source.UserStatSheet.Master)
            {
                dialog.Reply(source, "Come back when you have mastered your art.", "generic_learnskill_initial");
                return false;
            }

            if (!ValidateRequiredStats(source, dialog, requirements) || !ValidatePrerequisiteSkills(source, dialog, requirements) || !ValidatePrerequisiteSpells(source, dialog, requirements) || !ValidateAndTakeItemAndGoldRequirements(source, dialog, requirements))
                return false;

            return true;
        }

        private void HandleInvalidLearningAttempt(Dialog dialog, Aisling source, Skill skillToLearn, string reason)
        {
            dialog.ReplyToUnknownInput(source);

            Logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Skill,
                      Topics.Actions.Learn,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(Subject)
                  .WithProperty(Subject.DialogSource)
                  .WithProperty(source)
                  .WithProperty(skillToLearn)
                  .LogWarning(
                      "Aisling {@AislingName} tried to learn skill {@SkillName} but is {Reason}",
                      source.Name,
                      skillToLearn.Template.Name,
                      reason);
        }

        private bool ValidateRequiredStats(Aisling source, Dialog dialog, LearningRequirements requirements)
        {
            var requiredStats = requirements.RequiredStats;

            if (requiredStats != null)
            {
                if (requiredStats.Str > source.StatSheet.EffectiveStr)
                {
                    dialog.Reply(source, "Come back when you are stronger.", "generic_learnskill_initial");
                    return false;
                }

                if (requiredStats.Int > source.StatSheet.EffectiveInt)
                {
                    dialog.Reply(source, "Come back when you are smarter.", "generic_learnskill_initial");
                    return false;
                }

                if (requiredStats.Wis > source.StatSheet.EffectiveWis)
                {
                    dialog.Reply(source, "Come back when you are wiser.", "generic_learnskill_initial");
                    return false;
                }

                if (requiredStats.Con > source.StatSheet.EffectiveCon)
                {
                    dialog.Reply(source, "Come back when you are tougher.", "generic_learnskill_initial");
                    return false;
                }

                if (requiredStats.Dex > source.StatSheet.EffectiveDex)
                {
                    dialog.Reply(source, "Come back when you are more dexterous.", "generic_learnskill_initial");
                    return false;
                }
   
            }
            return true;
        }

        private bool ValidatePrerequisiteSkills(Aisling source, Dialog dialog, LearningRequirements requirements)
        {
            foreach (var requiredSkill in requirements.PrerequisiteSkills)
            {
                if (!source.SkillBook.TryGetObjectByTemplateKey(requiredSkill.TemplateKey, out var existingSkill))
                {
                    dialog.Reply(source, "Come back when you are more skillful.", "generic_learnskill_initial");
                    return false;
                }

                if (existingSkill.Level < requiredSkill.Level)
                {
                    dialog.Reply(source, "Come back when you are more skillful.", "generic_learnskill_initial");
                    return false;
                }
            }

            return true;
        }

        private bool ValidatePrerequisiteSpells(Aisling source, Dialog dialog, LearningRequirements requirements)
        {
            foreach (var requiredSpell in requirements.PrerequisiteSpells)
            {
                if (!source.SpellBook.TryGetObjectByTemplateKey(requiredSpell.TemplateKey, out var existingSpell))
                {
                    dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnskill_initial");
                    return false;
                }

                if (existingSpell.Level < requiredSpell.Level)
                {
                    dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnskill_initial");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateAndTakeItemAndGoldRequirements(Aisling source, Dialog dialog, LearningRequirements requirements)
        {
            foreach (var itemRequirement in requirements.ItemRequirements)
            {
                var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

                if (!source.Inventory.HasCount(requiredItem.DisplayName, itemRequirement.AmountRequired))
                {
                    dialog.Reply(source, "Come back when you have what is required.", "generic_learnskill_initial");
                    return false;
                }
            }
            
            if (requirements.RequiredGold.HasValue && (source.Gold < requirements.RequiredGold.Value))
            {
                dialog.Reply(source, "Come back when you are more wealthy.", "generic_learnskill_initial");
                return false;
            }

            foreach (var itemRequirement in requirements.ItemRequirements)
            {
                var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);
                source.Inventory.RemoveQuantity(requiredItem.DisplayName, itemRequirement.AmountRequired);
            }
            
            if (requirements.RequiredGold.HasValue)
                source.TryTakeGold(requirements.RequiredGold.Value);

            return true;
        }
    }
}
