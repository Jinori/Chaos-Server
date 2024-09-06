using Chaos.Common.Definitions;
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
    public class LearnSpellScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ILogger<LearnSpellScript> Logger;
        private readonly ISkillFactory SkillFactory;
        private readonly ISpellFactory SpellFactory;
        private readonly ISpellTeacherSource SpellTeacherSource;
        
        private bool IsPureMaster(Aisling source)
        {
            return source.Legend.ContainsKey("dedicated");
        }

        private readonly Dictionary<string, List<string>> SpellUpgradesByTemplateKey = new(StringComparer.OrdinalIgnoreCase)
        {
            // Wizard - Athar family
            { "beagathar", new List<string> { "athar", "morathar", "ardathar" } },
            { "athar", new List<string> { "morathar", "ardathar" } },
            { "morathar", new List<string> { "ardathar" } },
            { "atharmeall", new List<string> { "moratharmeall", "ardatharmeall" } },
            { "moratharmeall", new List<string> { "ardatharmeall" } },
            { "beagatharlamh", new List<string> { "atharlamh", "moratharlamh" } },
            { "atharlamh", new List<string> { "moratharlamh" } },

            // Wizard - Creag family
            { "beagcreag", new List<string> { "creag", "morcreag", "ardcreag" } },
            { "creag", new List<string> { "morcreag", "ardcreag" } },
            { "morcreag", new List<string> { "ardcreag" } },
            { "creagmeall", new List<string> { "morcreagmeall", "ardcreagmeall" } },
            { "morcreagmeall", new List<string> { "ardcreagmeall" } },
            { "beagcreaglamh", new List<string> { "creaglamh", "morcreaglamh" } },
            { "creaglamh", new List<string> { "morcreaglamh" } },

            // Wizard - Sal family
            { "beagsal", new List<string> { "sal", "morsal", "ardsal" } },
            { "sal", new List<string> { "morsal", "ardsal" } },
            { "morsal", new List<string> { "ardsal" } },
            { "salmeall", new List<string> { "morsalmeall", "ardsalmeall" } },
            { "morsalmeall", new List<string> { "ardsalmeall" } },
            { "beagsallamh", new List<string> { "sallamh", "morsallamh" } },
            { "sallamh", new List<string> { "morsallamh" } },

            // Wizard - Srad family
            { "beagsrad", new List<string> { "srad", "morsrad", "ardsrad" } },
            { "srad", new List<string> { "morsrad", "ardsrad" } },
            { "morsrad", new List<string> { "ardsrad" } },
            { "sradmeall", new List<string> { "morsradmeall", "ardatharmeall" } },
            { "morsradmeall", new List<string> { "ardsradmeall" } },
            { "beagsradlamh", new List<string> { "sradlamh", "morsradlamh" } },
            { "sradlamh", new List<string> { "morsradlamh" } },

            // Wizard - Arcane family
            { "arcanebolt", new List<string> { "arcanemissile", "arcaneblast", "arcaneexplosion" } },
            { "arcanemissile", new List<string> { "arcaneblast", "arcaneexplosion" } },
            { "arcaneblast", new List<string> { "arcaneexplosion" } },

            // Rogue - Trap family
            { "needletrap", new List<string> { "stilettotrap", "bolttrap", "coiledbolttrap", "springtrap", "maidentrap", "pitfalltrap" } },
            { "stilettotrap", new List<string> { "bolttrap", "coiledbolttrap", "springtrap", "maidentrap", "pitfalltrap" } },
            { "bolttrap", new List<string> { "coiledbolttrap", "springtrap", "maidentrap", "pitfalltrap" } },
            { "coiledbolttrap", new List<string> { "springtrap", "maidentrap", "pitfalltrap" } },
            { "springtrap", new List<string> { "maidentrap", "pitfalltrap" } },
            { "maidentrap", new List<string> { "pitfalltrap" } },

            // Priest
            { "beagpramh", new List<string> { "pramh" } },
            { "beothaich", new List<string> { "revive" } },
            { "revive", new List<string> {"resurrection"} },

            // Warrior
            { "battlecry", new List<string> { "warcry" } },

            // Monk
            { "goad", new List<string> { "howl", "roar" } },
            { "howl", new List<string> { "roar" } },
            { "miststance", new List<string> { "tidestance" } },
            { "smokestance", new List<string> { "flamestance" } },
            { "earthenstance", new List<string> { "rockstance" } },
            { "thunderstance", new List<string> { "lightningstance" } },
        };
        
        private readonly Dictionary<string, List<string>> PureAbilities = new()
        {
            { "magmasurge", new List<string> { "magmasurge" } },
            { "tidalbreeze", new List<string> { "tidalbreeze" } },
            { "grouphide", new List<string> { "grouphide" } },
            { "healzone", new List<string> { "healzone" } },
            { "darkstorm", new List<string> { "darkstorm" } },
        };

        public LearnSpellScript(
            Dialog subject,
            IItemFactory itemFactory,
            ISkillFactory skillFactory,
            ISpellFactory spellFactory,
            ILogger<LearnSpellScript> logger
        )
            : base(subject)
        {
            ItemFactory = itemFactory;
            SkillFactory = skillFactory;
            SpellFactory = spellFactory;
            Logger = logger;
            SpellTeacherSource = (ISpellTeacherSource)Subject.DialogSource;
        }

        public override void OnDisplaying(Aisling source)
        {
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "generic_learnspell_initial":
                    OnDisplayingInitial(source);
                    break;
                case "generic_learnspell_showrequirements":
                    OnDisplayingRequirements(source);
                    break;
                case "generic_learnspell_accepted":
                    OnDisplayingAccepted(source);
                    break;
            }
        }

        private void OnDisplayingAccepted(Aisling source)
        {
            if (!TryFetchArgs<string>(out var spellName)
                || !TryGetSpell(spellName, source, out var spell)
                || source.SpellBook.Contains(spellName))
            {
                Subject.ReplyToUnknownInput(source);
                return;
            }

            if (!ValidateAndTakeRequirements(source, Subject, spell))
                return;

            var spellToLearn = SpellFactory.Create(spell.Template.TemplateKey);

            if (!string.IsNullOrEmpty(spellToLearn.Template.LearningRequirements?.SkillSpellToUpgrade))
            {
                var oldSpell = source.SpellBook.FirstOrDefault(
                    s => s.Template.TemplateKey.Equals(
                        spellToLearn.Template.LearningRequirements?.SkillSpellToUpgrade,
                        StringComparison.OrdinalIgnoreCase));

                if (oldSpell != null)
                {
                    source.SpellBook.RemoveByTemplateKey(oldSpell.Template.TemplateKey);
                    source.SendOrangeBarMessage($"{oldSpell.Template.Name} has been upgraded to {spellToLearn.Template.Name}.");
                }
            }

            var learnSpellResult = ComplexActionHelper.LearnSpell(source, spellToLearn);

            switch (learnSpellResult)
            {
                case ComplexActionHelper.LearnSpellResult.Success:
                    Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Spell, Topics.Actions.Learn)
                          .WithProperty(Subject)
                          .WithProperty(Subject.DialogSource)
                          .WithProperty(source)
                          .WithProperty(spell)
                          .LogInformation("Aisling {@AislingName} learned spell {@SpellName}", source.Name, spell.Template.Name);

                    var animation = new Animation
                    {
                        AnimationSpeed = 50,
                        TargetAnimation = 22
                    };

                    source.Animate(animation);

                    break;
                case ComplexActionHelper.LearnSpellResult.NoRoom:
                    Subject.Reply(
                        source,
                        "Like the spilled contents of an unbalanced cup, some knowledge is best forgotten...",
                        "generic_learnspell_initial");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisplayingInitial(Aisling source)
        {
            Subject.Spells.Clear();

            foreach (var spell in SpellTeacherSource.SpellsToTeach)
            {
                if (!IsSpellLearnable(spell, source))
                    continue;

                Subject.Spells.Add(spell);
            }
        }

        private bool IsSpellLearnable(Spell spell, Aisling source)
        {
            var requiredBaseClass = spell.Template.Class;
            var requiredAdvClass = spell.Template.AdvClass;

            if (requiredBaseClass.HasValue && !source.HasClass(requiredBaseClass.Value))
            {
                // Base class requirement not met
                return false;
            }

            if (requiredAdvClass.HasValue && (requiredAdvClass.Value != source.UserStatSheet.AdvClass))
            {
                // Advanced class requirement not met
                return false;
            }

            if (source.SpellBook.Contains(spell))
            {
                // Spell already in spellbook
                return false;
            }
            
            if (PureAbilities.ContainsKey(spell.Template.TemplateKey.ToLower()) && IsPureMaster(source))
            {
                return false;
            }
            
            if (source.HasClass(BaseClass.Wizard) && (spell.Template.WizardElement != WizardElement.None))
                if (source.Trackers.Flags.TryGetFlag(out WizardElement currentElements))
                    if ((currentElements & spell.Template.WizardElement) == 0)
                        return false;

            // Check if the current spell is part of an upgrade chain
            if (SpellUpgradesByTemplateKey.Values.Any(upgrades =>
                    upgrades.Contains(spell.Template.TemplateKey, StringComparer.OrdinalIgnoreCase)))
            {
                var baseSpellKey = SpellUpgradesByTemplateKey
                    .FirstOrDefault(kvp =>
                        kvp.Value.Contains(spell.Template.TemplateKey, StringComparer.OrdinalIgnoreCase))
                    .Key;

                // Find the index of the current spell in its upgrade chain (case-insensitive)
                var upgradeIndex = SpellUpgradesByTemplateKey[baseSpellKey]
                    .FindIndex(u => string.Equals(u, spell.Template.TemplateKey, StringComparison.OrdinalIgnoreCase));

                switch (upgradeIndex)
                {
                    case -1:
                        // Spell not found in the upgrade chain
                        return false;
                    case > 0:
                    {
                        var precedingSpell = SpellUpgradesByTemplateKey[baseSpellKey][upgradeIndex - 1];

                        // Check if the preceding spell is known
                        if (source.SpellBook.All(s =>
                                !string.Equals(s.Template.TemplateKey, precedingSpell, StringComparison.OrdinalIgnoreCase)))
                        {
                            // Immediate preceding spell in the upgrade chain not known
                            return false;
                        }

                        break;
                    }
                    default:
                    {
                        // If the spell is the first upgrade, ensure the base spell is known
                        if (source.SpellBook.All(s =>
                                !string.Equals(s.Template.TemplateKey, baseSpellKey, StringComparison.OrdinalIgnoreCase)))
                        {
                            // Base spell not known
                            return false;
                        }

                        break;
                    }
                }
            }

            // Ensure that only the base spell is shown if neither the base spell nor the upgrade is known
            if (SpellUpgradesByTemplateKey.TryGetValue(spell.Template.TemplateKey, out var upgrades))
            {
                // If the player knows any upgrades in the chain, do not show the base spell
                if (upgrades.Any(upgrade => source.SpellBook.Any(s =>
                        string.Equals(s.Template.TemplateKey, upgrade, StringComparison.OrdinalIgnoreCase))))
                {
                    // Known upgrade exists in spellbook
                    return false;
                }
            }

            // Spell is learnable
            return true;
        }


        private void OnDisplayingRequirements(Aisling source)
        {
            if (!TryFetchArgs<string>(out var spellName)
                || !TryGetSpell(spellName, source, out var spell)
                || source.SpellBook.Contains(spellName))
            {
                Subject.ReplyToUnknownInput(source);
                return;
            }

            var learningRequirementsStr = spell.Template
                                               .LearningRequirements
                                               ?.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory)
                                               .ToString();

            Subject.InjectTextParameters(spell.Template.Description ?? string.Empty, learningRequirementsStr ?? string.Empty);
        }

        private bool TryGetSpell(string spellName, Aisling source, [MaybeNullWhen(false)] out Spell spell)
        {
            //name matches
            //source has the spell's class
            //adv class matches if there is one
            spell = SpellTeacherSource.SpellsToTeach.FirstOrDefault(
                spell => spell.Template.Name.EqualsI(spellName)
                         && source.HasClass(spell.Template.Class!.Value)
                         && (!spell.Template.AdvClass.HasValue || source.UserStatSheet.AdvClass == spell.Template.AdvClass.Value));

            return spell != null;
        }

        private bool HasSpellOrUpgrade(Aisling source, string spellTemplateKey)
        {
            if (source.SpellBook.Any(s => s.Template.TemplateKey.Equals(spellTemplateKey, StringComparison.OrdinalIgnoreCase)))
                return true;

            if (SpellUpgradesByTemplateKey.TryGetValue(spellTemplateKey, out var upgrades))
                return upgrades.Any(
                    upgrade => source.SpellBook.Any(s => s.Template.TemplateKey.Equals(upgrade, StringComparison.OrdinalIgnoreCase)));

            return false;
        }

        public bool ValidateAndTakeRequirements(Aisling source, Dialog dialog, Spell spellToLearn)
        {
            var template = spellToLearn.Template;
            var requirements = template.LearningRequirements;

            if (requirements == null)
                return true;

            if (template.Class.HasValue && !source.HasClass(template.Class.Value))
            {
                dialog.ReplyToUnknownInput(source);

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Spell,
                          Topics.Actions.Learn,
                          Topics.Qualifiers.Cheating)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(spellToLearn)
                      .LogWarning(
                          "Aisling {@AislingName} tried to learn spell {@SpellName} but is not the correct class (possibly packeting)",
                          source.Name,
                          template.Name);

                return false;
            }

            if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
            {
                dialog.ReplyToUnknownInput(source);

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Spell,
                          Topics.Actions.Learn,
                          Topics.Qualifiers.Cheating)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(spellToLearn)
                      .LogWarning(
                          "Aisling {@AislingName} tried to learn spell {@SpellName} but is not the correct adv class (possibly packeting)",
                          source.Name,
                          template.Name);

                return false;
            }

            if (source.StatSheet.Level < template.Level)
            {
                dialog.Reply(source, "Come back when you are more experienced.", "generic_learnspell_initial");

                return false;
            }

            if (template.RequiresMaster && !source.UserStatSheet.Master)
            {
                dialog.Reply(source, "Come back when you have mastered your art.", "generic_learnspell_initial");

                return false;
            }

            if (requirements.RequiredStats != null)
            {
                var requiredStats = requirements.RequiredStats;

                if (requiredStats.Str > source.StatSheet.EffectiveStr)
                {
                    dialog.Reply(source, "Come back when you are stronger.", "generic_learnspell_initial");

                    return false;
                }

                if (requiredStats.Int > source.StatSheet.EffectiveInt)
                {
                    dialog.Reply(source, "Come back when you are smarter.", "generic_learnspell_initial");

                    return false;
                }

                if (requiredStats.Wis > source.StatSheet.EffectiveWis)
                {
                    dialog.Reply(source, "Come back when you are wiser.", "generic_learnspell_initial");

                    return false;
                }

                if (requiredStats.Con > source.StatSheet.EffectiveCon)
                {
                    dialog.Reply(source, "Come back when you are tougher.", "generic_learnspell_initial");

                    return false;
                }

                if (requiredStats.Dex > source.StatSheet.EffectiveDex)
                {
                    dialog.Reply(source, "Come back when you are more dexterous.", "generic_learnspell_initial");

                    return false;
                }
            }

            foreach (var requiredSkill in requirements.PrerequisiteSkills)
            {
                if (!source.SkillBook.TryGetObjectByTemplateKey(requiredSkill.TemplateKey, out var existingSkill))
                {
                    dialog.Reply(source, "Come back when you are more skillful.", "generic_learnspell_initial");

                    return false;
                }

                if (existingSkill.Level < requiredSkill.Level)
                {
                    dialog.Reply(source, "Come back when you are more skillful.", "generic_learnspell_initial");

                    return false;
                }
            }

            foreach (var requiredSpell in requirements.PrerequisiteSpells)
            {
                if (!HasSpellOrUpgrade(source, requiredSpell.TemplateKey))
                {
                    dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnspell_initial");

                    return false;
                }

                var existingSpell = source.SpellBook.FirstOrDefault(
                    s => s.Template.TemplateKey.Equals(requiredSpell.TemplateKey, StringComparison.OrdinalIgnoreCase));

                if (existingSpell != null && existingSpell.Level < requiredSpell.Level)
                {
                    dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnspell_initial");

                    return false;
                }
            }

            foreach (var itemRequirement in requirements.ItemRequirements)
            {
                var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

                if (!source.Inventory.HasCount(requiredItem.DisplayName, itemRequirement.AmountRequired))
                {
                    dialog.Reply(source, "Come back when you have what is required.", "generic_learnspell_initial");

                    return false;
                }
            }

            if (requirements.RequiredGold.HasValue && (source.Gold < requirements.RequiredGold.Value))
            {
                dialog.Reply(source, "Come back when you are more wealthy.", "generic_learnspell_initial");

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
