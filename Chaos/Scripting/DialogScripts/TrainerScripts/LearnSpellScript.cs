using Chaos.Common.Definitions;
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
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.TrainerScripts;

public class LearnSpellScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<LearnSpellScript> Logger;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly ISpellTeacherSource SpellTeacherSource;

    private readonly Dictionary<string, List<string>> SpellUpgrades = new()
    {
        //Wizard
        { "Beag Sal", new List<string> { "Sal", "Mor Sal", "Ard Sal" } },
        { "Sal", new List<string> { "Mor Sal", "Ard Sal" } },
        { "Mor Sal", new List<string> { "Ard Sal" } },
        { "Sal Meall", new List<string> { "Mor Sal Meall" } },
        { "Beag Creag", new List<string> { "Creag", "Mor Creag", "Ard Creag" } },
        { "Creag", new List<string> { "Mor Creag", "Ard Creag" } },
        { "Creag Meall", new List<string> { "Mor Creag Meall" } },
        { "Mor Creag", new List<string> { "Ard Creag" } },
        { "Beag Athar", new List<string> { "Athar", "Mor Athar", "Ard Athar" } },
        { "Athar", new List<string> { "Mor Athar", "Ard Athar" } },
        { "Athar Meall", new List<string> { "Mor Athar Meall" } },
        { "Mor Athar", new List<string> { "Ard Athar" } },
        { "Beag Srad", new List<string> { "Srad", "Mor Srad", "Ard Srad" } },
        { "Srad Meall", new List<string> { "Mor Srad Meall" } },
        { "Srad", new List<string> { "Mor Srad", "Ard Srad" } },
        
        { "Mor Srad", new List<string> { "Ard Srad" } },
        { "Beag Pramh", new List<string> { "Pramh" } },
        { "Beag Srad Lamh", new List<string> { "Srad Lamh" } },
        { "Beag Sal Lamh", new List<string> { "Sal Lamh" } },
        { "Beag Creag Lamh", new List<string> { "Creag Lamh" } },
        { "Beag Athar Lamh", new List<string> { "Athar Lamh" } },
        //Warrior
        { "Battle Cry", new List<string> { "War Cry" } },
        //Rogue
        { "Needle Trap", new List<string> { "Stiletto Trap", "Bolt Trap", "Coiled Bolt Trap", "Spring Trap", "Maiden Trap" } },
        { "Stiletto", new List<string> { "Bolt Trap", "Coiled Bolt Trap", "Spring Trap", "Maiden Trap" } },
        { "Bolt Trap", new List<string> { "Coiled Bolt Trap", "Spring Trap", "Maiden Trap" } },
        { "Coiled Bolt Trap", new List<string> { "Spring Trap", "Maiden Trap" } },
        { "Spring Trap", new List<string> { "Maiden Trap" } },
        //Monk
        { "Goad", new List<string> { "Howl" } }
    };

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_learnspell_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_learnspell_showrequirements":
            {
                OnDisplayingRequirements(source);

                break;
            }
            case "generic_learnspell_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string>(out var spellName)
            || !SpellTeacherSource.TryGetSpell(spellName, out var spell)
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
        Subject.Spells = new List<Spell>();

        foreach (var spell in SpellTeacherSource.SpellsToTeach)
        {
            var requiredBaseClass = spell.Template.Class;
            var requiredAdvClass = spell.Template.AdvClass;

            // If this spell is not available to the player's class, skip it.
            if (requiredBaseClass.HasValue && !source.HasClass(requiredBaseClass.Value))
                continue;

            // If this spell is not available to the player's adv class, skip it.
            if (requiredAdvClass.HasValue && (requiredAdvClass.Value != source.UserStatSheet.AdvClass))
                continue;

            // If the player already knows this spell, skip it.
            if (source.SpellBook.Contains(spell))
                continue;

            
            //Try this Nate
            /*if (source.HasClass(BaseClass.Wizard) && (spell.Template.WizardElement != WizardElement.None))
            {
                if (source.Trackers.Flags.TryGetFlag(out WizardElement currentElements))
                {
                    if ((currentElements & spell.Template.WizardElement) == 0) // If the Wizard does not have the element of the spell
                        continue;
                }
            }*/
            
            // Check if the source has the Wizard class and the spell also has a Wizard element, then check if the source has a matching
            // Wizard Element flag. If not, skip the spell.
            if (source.HasClass(BaseClass.Wizard) && (spell.Template.WizardElement != null))
                if (source.Trackers.Flags.TryGetFlag(out WizardElement ele))
                    if (spell.Template.WizardElement != ele)
                        continue;
            
            // Check if the player's spellbook contains any spells that are upgrades of the spell they're trying to learn.
            var knowsUpgrade = source.SpellBook.Any(
                s => SpellUpgrades.ContainsKey(spell.Template.Name) && SpellUpgrades[spell.Template.Name].Contains(s.Template.Name));

            // If the player knows an upgrade of the spell they're trying to learn, skip it.
            if (knowsUpgrade)
                continue;

            Subject.Spells.Add(spell);
        }
    }

    private void OnDisplayingRequirements(Aisling source)
    {
        if (!TryFetchArgs<string>(out var spellName)
            || !SpellTeacherSource.TryGetSpell(spellName, out var spell)
            || source.SpellBook.Contains(spellName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var learningRequirementsStr = spell.Template.LearningRequirements?.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory)
                                           .ToString();

        Subject.InjectTextParameters(spell.Template.Description ?? string.Empty, learningRequirementsStr ?? string.Empty);
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

        foreach (var skillTemplateKey in requirements.PrerequisiteSkillTemplateKeys)
        {
            var requiredSkill = SkillFactory.CreateFaux(skillTemplateKey);

            if (!source.SkillBook.Contains(requiredSkill))
            {
                dialog.Reply(source, "Come back when you are more skillful.", "generic_learnspell_initial");

                return false;
            }
        }

        foreach (var spellTemplateKey in requirements.PrerequisiteSpellTemplateKeys)
        {
            var requiredSpell = SpellFactory.CreateFaux(spellTemplateKey);

            if (!source.SpellBook.Contains(requiredSpell))
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

    public sealed class TieredSpell
    {
        public string Name { get; }
        public string TemplateKey { get; }
        public int Tier { get; }

        public TieredSpell(string templateKey, string name, int tier)
        {
            TemplateKey = templateKey;
            Name = name;
            Tier = tier;
        }
    }
}