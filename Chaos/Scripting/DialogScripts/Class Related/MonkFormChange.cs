using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class MonkFormChange : DialogScriptBase
{
    private readonly ILogger<MonkFormChange> Logger;

    public MonkFormChange(Dialog subject, ILogger<MonkFormChange> logger)
        : base(subject) =>
        Logger = logger;

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
        { MonkElementForm.Air,   ["thunderstance", "lightningstance"] },
        { MonkElementForm.Fire,  ["smokestance", "flamestance"] },
        { MonkElementForm.Earth, ["earthenstance", "rockstance"] },
        { MonkElementForm.Water, ["miststance", "tidestance"] }
    };

    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Template.TemplateKey.Equals("ironfoot_initial", StringComparison.OrdinalIgnoreCase))
        {
            if (!source.Trackers.Enums.TryGetValue(out MonkElementForm _))
            {
                Subject.Reply(source, "You need to learn a form before I can help you, little rabbit.");

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Learn, Topics.Entities.Dialog)
                      .WithProperty(source)
                      .LogInformation("{@AislingName} attempted to interact with Ironfoot without a monk form", source.Name);
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is not 1)
            return;

        var formConfirmMappings = new Dictionary<string, MonkElementForm>(StringComparer.OrdinalIgnoreCase)
        {
            { "ironfoot_fireconfirm", MonkElementForm.Fire },
            { "ironfoot_waterconfirm", MonkElementForm.Water },
            { "ironfoot_earthconfirm", MonkElementForm.Earth },
            { "ironfoot_airconfirm", MonkElementForm.Air }
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
        source.Legend.AddUnique(new LegendMark(
            $"{newForm.Humanize().Titleize()} Elemental Dedication",
            $"{newForm}Monk",
            MarkIcon.Monk,
            MarkColor.Blue,
            1,
            GameTime.Now
        ));

        source.SendServerMessage(
            ServerMessageType.ActiveMessage,
            $"You have dissolved yourself of {oldForm} and have chosen {newForm.Humanize().Titleize()}."
        );

        source.Trackers.Enums.Set(typeof(MonkElementForm), newForm);

        if (FormAnimations.TryGetValue(newForm, out var animation))
            source.Animate(animation);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
              .WithProperty(source)
              .LogInformation("{@AislingName} has dedicated to the {NewForm} form", source.Name, newForm);
    }
}
