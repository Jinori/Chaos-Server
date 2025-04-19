using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class SubFormChange : DialogScriptBase
{
    private readonly ILogger<SubFormChange> Logger;
    private readonly ISpellFactory SpellFactory;
    private readonly ISkillFactory SkillFactory;

    public SubFormChange(Dialog subject, ILogger<SubFormChange> logger, ISpellFactory spellFactory, ISkillFactory skillFactory)
        : base(subject)
    {
        Logger = logger;
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
    }

    private readonly Dictionary<MonkElementForm, Animation> FormAnimations = new()
    {
        { MonkElementForm.Water, new Animation { TargetAnimation = 67, AnimationSpeed = 100 } },
        { MonkElementForm.Fire,  new Animation { TargetAnimation = 102, AnimationSpeed = 100 } },
        { MonkElementForm.Air,   new Animation { TargetAnimation = 156, AnimationSpeed = 100 } },
        { MonkElementForm.Earth, new Animation { TargetAnimation = 87, AnimationSpeed = 100 } }
    };

    private readonly Dictionary<MonkElementForm, HashSet<string>> SkillSets = new()
    {
        { MonkElementForm.Air,   ["airpunch", "tempestkick"] },
        { MonkElementForm.Fire,  ["firepunch", "dracotailkick"] },
        { MonkElementForm.Earth, ["earthpunch", "seismickick"] },
        { MonkElementForm.Water, ["waterpunch", "tsunamikick"] }
    };

    private readonly Dictionary<MonkElementForm, HashSet<string>> SpellSets = new()
    {
        { MonkElementForm.Air,   ["thunderstance"]},
        { MonkElementForm.Fire,  ["smokestance"] },
        { MonkElementForm.Earth, ["earthenstance"] },
        { MonkElementForm.Water, ["miststance"] }
    };

    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Template.TemplateKey.EqualsI("elise_initial"))
        {
            if (!source.Trackers.Enums.TryGetValue(out MonkElementForm _))
            {
                Subject.Reply(source, "You need to learn a form before I can help you, little rabbit.");

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Learn, Topics.Entities.Dialog)
                      .WithProperty(source)
                      .LogInformation("{@AislingName} attempted to interact with Elise without a monk form", source.Name);
            }
            
        }

        if (Subject.Template.TemplateKey.EqualsI("elise_changeelementalform"))
        {
            if (source.Trackers.Enums.TryGetValue(out MonkElementForm oldForm))
            {
                switch (oldForm)
                {
                    case MonkElementForm.Water:
                        RemoveOption(Subject, "Rededicate Water");
                        break;
                    case MonkElementForm.Earth:
                        RemoveOption(Subject, "Rededicate Earth");
                        break;
                    case MonkElementForm.Air:
                        RemoveOption(Subject, "Rededicate Air");
                        break;
                    case MonkElementForm.Fire:
                        RemoveOption(Subject, "Rededicate Fire");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }   
        }
    }

    
    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName)
                   .HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }
    
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.Template.TemplateKey.EndsWith("confirm", StringComparison.Ordinal))
            return;

        if (!source.Legend.ContainsKey("dedicated"))
        {
            Subject.Reply(source, "You are not rededicated.");
            return;   
        }

        if (source.UserStatSheet.BaseClass == BaseClass.Monk)
            Subject.Reply(source, "You are a monk. Please see Leadfoot");

        var formConfirmMappings = new Dictionary<string, MonkElementForm>(StringComparer.OrdinalIgnoreCase)
        {
            { "elise_fireconfirm", MonkElementForm.Fire },
            { "elise_waterconfirm", MonkElementForm.Water },
            { "elise_earthconfirm", MonkElementForm.Earth },
            { "elise_airconfirm", MonkElementForm.Air }
        };

        if (formConfirmMappings.TryGetValue(Subject.Template.TemplateKey, out var newForm))
        {
            if (source.Trackers.Enums.TryGetValue(out MonkElementForm oldForm))
            {
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                      .WithProperty(source)
                      .LogInformation("{@AislingName} is changing Monk form from {OldForm} to {NewForm}", source.Name, oldForm, newForm);

                RemoveOldForm(source, oldForm);
                GiveNewForm(source, oldForm, newForm);
            }
            else
            {
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote, Topics.Entities.Dialog)
                      .WithProperty(source)
                      .LogWarning("{@AislingName} attempted to change Monk form but had no existing form", source.Name);
            }
        }
    }

    public void RemoveOldForm(Aisling source, MonkElementForm oldForm)
    {
        source.Legend.Remove($"{oldForm}Monk", out _);

        if (SkillSets.TryGetValue(oldForm, out var skillsToRemove))
        {
            var removedSkills = source.SkillBook
                                      .Where(skill => skillsToRemove.Contains(skill.Template.TemplateKey.ToLowerInvariant()))
                                      .Select(skill => skill.Template.TemplateKey)
                                      .ToList();

            foreach (var skillKey in removedSkills)
                source.SkillBook.RemoveByTemplateKey(skillKey);

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} lost skills: {SkillKeys}", source.Name, removedSkills);
        }

        if (SpellSets.TryGetValue(oldForm, out var spellsToRemove))
        {
            var removedSpells = source.SpellBook
                                      .Where(spell => spellsToRemove.Contains(spell.Template.TemplateKey.ToLowerInvariant()))
                                      .Select(spell => spell.Template.TemplateKey)
                                      .ToList();

            foreach (var spellKey in removedSpells)
                source.SpellBook.RemoveByTemplateKey(spellKey);

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} lost spells: {SpellKeys}", source.Name, removedSpells);
        }
    }

    public void GiveNewForm(Aisling source, MonkElementForm oldForm, MonkElementForm newForm)
    {
        source.Legend.AddUnique(
            new LegendMark(
                $"{newForm.Humanize().Titleize()} Elemental Dedication",
                $"{newForm}Monk",
                MarkIcon.Monk,
                MarkColor.Blue,
                1,
                GameTime.Now));

        source.SendServerMessage(
            ServerMessageType.ActiveMessage,
            $"You have dissolved yourself of {oldForm.Humanize().Titleize()} and have chosen {newForm.Humanize().Titleize()}.");

        source.Trackers.Enums.Set(typeof(MonkElementForm), newForm);

        if (FormAnimations.TryGetValue(newForm, out var animation))
            source.Animate(animation);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
              .WithProperty(source)
              .LogInformation("{@AislingName} has dedicated to the {NewForm} form", source.Name, newForm);

        // Grant new skills
        if (SkillSets.TryGetValue(newForm, out var newSkills))
        {
            foreach (var skillKey in newSkills)
            {
                var skill = SkillFactory.Create(skillKey);
                var added = source.SkillBook.TryAddToNextSlot(skill);

                if (added)
                {
                    Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Learn)
                          .WithProperty(source)
                          .LogInformation("{@AislingName} learned new skill: {SkillKey}", source.Name, skillKey);
                }
            }
        }
        else
        {
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Learn, Topics.Entities.Dialog)
                  .WithProperty(source)
                  .LogWarning("Skill set not found for new monk form: {NewForm}", newForm);
        }

        // Grant new spells
        if (SpellSets.TryGetValue(newForm, out var newSpells))
        {
            foreach (var spellKey in newSpells)
            {
                var spell = SpellFactory.Create(spellKey);
                var added = source.SpellBook.TryAddToNextSlot(spell);

                if (added)
                {
                    Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Learn)
                          .WithProperty(source)
                          .LogInformation("{@AislingName} learned new spell: {SpellKey}", source.Name, spellKey);
                }
            }
        }
        else
        {
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Learn, Topics.Entities.Dialog)
                  .WithProperty(source)
                  .LogWarning("Spell set not found for new monk form: {NewForm}", newForm);
        }
    }
}
