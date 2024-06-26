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


namespace Chaos.Scripting.DialogScripts.TrainerScripts;

public class LearnSpellScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<LearnSpellScript> Logger;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly ISpellTeacherSource SpellTeacherSource;

    private readonly Dictionary<string, List<string>> SpellUpgradesByTemplateKey = new(StringComparer.OrdinalIgnoreCase)
    {
        // Wizard - Athar family
        { "beagathar", ["athar", "morathar", "ardathar"] },
        { "athar", ["morathar", "ardathar"] },
        { "morathar", ["ardathar"] },
        { "atharmeall", ["moratharmeall"] },
        { "beagatharlamh", ["atharlamh", "moratharlamh"] },
        { "atharlamh", ["moratharlamh"] },
        { "moratharmeall", ["ardatharmeall"] },

        // Wizard - Creag family
        { "beagcreag", ["creag", "morcreag", "ardcreag"] },
        { "creag", ["morcreag", "ardcreag"] },
        { "morcreag", ["ardcreag"] },
        { "creagmeall", ["morcreagmeall"] },
        { "beagcreaglamh", ["creaglamh", "morcreaglamh"] },
        { "creaglamh", ["morcreaglamh"] },

        // Wizard - Sal family
        { "beagsal", ["sal", "morsal", "ardsal"] },
        { "sal", ["morsal", "ardsal"] },
        { "morsal", ["ardsal"] },
        { "salmeall", ["morsalmeall"] },
        { "beagsallamh", ["sallamh", "morsallamh"] },
        { "sallamh", ["morsallamh"] },

        // Wizard - Srad family
        { "beagsrad", ["srad", "morsrad", "ardsrad"] },
        { "srad", ["morsrad", "ardsrad"] },
        { "morsrad", ["ardsrad"] },
        { "sradmeall", ["morsradmeall"] },
        { "beagsradlamh", ["sradlamh", "morsradlamh"] },
        { "sradlamh", ["morsradlamh"] },

        // Wizard - Arcane family
        { "arcanebolt", ["arcanemissile", "arcaneblast"] },
        { "arcanemissile", ["arcaneblast"] },

        // Rogue - Trap family
        { "needletrap", ["stilettotrap", "bolttrap", "coiledbolttrap", "springtrap", "maidentrap"] },
        { "stilettotrap", ["bolttrap", "coiledbolttrap", "springtrap", "maidentrap"] },
        { "bolttrap", ["coiledbolttrap", "springtrap", "maidentrap"] },
        { "coiledbolttrap", ["springtrap", "maidentrap"] },
        { "springtrap", ["maidentrap"] },

        // Priest
        { "beagpramh", ["pramh"] },
        { "beothaich", ["revive"] },

        //Warrior
        { "battlecry", ["warcry"] },

        //Monk
        { "goad", ["howl"] }
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
                s => s.Template.Name.Equals(
                    spellToLearn.Template.LearningRequirements?.SkillSpellToUpgrade,
                    StringComparison.OrdinalIgnoreCase));

            if (oldSpell != null)
            {
                source.SpellBook.Remove(oldSpell.Template.Name);
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
        Subject.Spells = [];

        foreach (var spell in SpellTeacherSource.SpellsToTeach)
        {
            var requiredBaseClass = spell.Template.Class;
            var requiredAdvClass = spell.Template.AdvClass;

            if (requiredBaseClass.HasValue && !source.HasClass(requiredBaseClass.Value))
                continue;

            if (requiredAdvClass.HasValue && (requiredAdvClass.Value != source.UserStatSheet.AdvClass))
                continue;

            if (source.SpellBook.Contains(spell))
                continue;

            if (source.HasClass(BaseClass.Wizard) && (spell.Template.WizardElement != WizardElement.None))
                if (source.Trackers.Flags.TryGetFlag(out WizardElement currentElements))
                    if ((currentElements & spell.Template.WizardElement) == 0)
                        continue;

            var knowsUpgrade = source.SpellBook.Any(
                s => SpellUpgradesByTemplateKey.ContainsKey(spell.Template.TemplateKey)
                     && SpellUpgradesByTemplateKey[spell.Template.TemplateKey]
                         .Contains(s.Template.TemplateKey, StringComparer.OrdinalIgnoreCase));

            if (knowsUpgrade)
                continue;

            Subject.Spells.Add(spell);
        }
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
                     && (!spell.Template.AdvClass.HasValue || (source.UserStatSheet.AdvClass == spell.Template.AdvClass.Value)));

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
                dialog.Reply(source, "Come back when you are more skillful.", "generic_learnskill_initial");

                return false;
            }

            if (existingSkill.Level < requiredSkill.Level)
            {
                dialog.Reply(source, "Come back when you are more skillful.", "generic_learnskill_initial");

                return false;
            }
        }

        foreach (var requiredSpell in requirements.PrerequisiteSpells)
        {
            if (!HasSpellOrUpgrade(source, requiredSpell.TemplateKey))
            {
                dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnskill_initial");

                return false;
            }

            var existingSpell = source.SpellBook.FirstOrDefault(
                s => s.Template.TemplateKey.Equals(requiredSpell.TemplateKey, StringComparison.OrdinalIgnoreCase));

            if (existingSpell != null && existingSpell.Level < requiredSpell.Level)
            {
                dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnskill_initial");

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