using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class MonkFormChange : DialogScriptBase
{
    /// <inheritdoc />
    public MonkFormChange(Dialog subject)
        : base(subject) { }

    private readonly Dictionary<MonkElementForm, Animation> FormAnimations = new()
    {
        { MonkElementForm.Water, new Animation { TargetAnimation = 67, AnimationSpeed = 100 } },
        { MonkElementForm.Fire, new Animation { TargetAnimation = 102, AnimationSpeed = 100 } },
        { MonkElementForm.Air, new Animation { TargetAnimation = 156, AnimationSpeed = 100 } },
        { MonkElementForm.Earth, new Animation { TargetAnimation = 87, AnimationSpeed = 100 } }
    };

    private readonly Dictionary<MonkElementForm, HashSet<string>> SkillSets = new()
    {
        { MonkElementForm.Air, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "airpunch", "tempestkick" } },
        { MonkElementForm.Fire, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "firepunch", "dracotailkick" } },
        { MonkElementForm.Earth, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "earthpunch", "seismickick" } },
        { MonkElementForm.Water, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "waterpunch", "tsunamikick" } }
    };

    private readonly Dictionary<MonkElementForm, HashSet<string>> SpellSets = new()
    {
        { MonkElementForm.Air, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "thunderstance", "lightningstance" } },
        { MonkElementForm.Fire, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "smokestance", "flamestance" } },
        { MonkElementForm.Earth, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "earthenstance", "rockstance" } },
        { MonkElementForm.Water, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "miststance", "tidestance" } }
    };


    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Template.TemplateKey.Equals("ironfoot_initial", StringComparison.OrdinalIgnoreCase))
        {
            if (!source.Trackers.Enums.TryGetValue(out MonkElementForm _))
                Subject.Reply(source, "You need to learn a form before I can help you, little rabbit.");
        }
    }

    public void RemoveOldForm(Aisling source, MonkElementForm oldForm)
    {
        source.Legend.Remove($"{oldForm}Monk", out _);

        if (SkillSets.TryGetValue(oldForm, out var skillsToRemove))
        {
            var skills = source.SkillBook
                .Where(skill => skillsToRemove.Contains(skill.Template.TemplateKey.ToLowerInvariant()))
                .Select(skill => skill.Template.TemplateKey)
                .ToList();

            foreach (var skillKey in skills)
                source.SkillBook.RemoveByTemplateKey(skillKey);
        }

        if (SpellSets.TryGetValue(oldForm, out var spellsToRemove))
        {
            var spells = source.SpellBook
                .Where(spell => spellsToRemove.Contains(spell.Template.TemplateKey.ToLowerInvariant()))
                .Select(spell => spell.Template.TemplateKey)
                .ToList();

            foreach (var spellKey in spells)
                source.SpellBook.RemoveByTemplateKey(spellKey);
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
            GameTime.Now));

        source.SendServerMessage(ServerMessageType.ActiveMessage,
            $"You have dissolved yourself of {oldForm} and have chosen {newForm.Humanize().Titleize()}.");

        source.Trackers.Enums.Set(typeof(MonkElementForm), newForm);

        if (FormAnimations.TryGetValue(newForm, out var animation))
            source.Animate(animation);
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is not 1) return;

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
                RemoveOldForm(source, oldForm);
                GiveNewForm(source, oldForm, newForm);
            }
        }
    }
}
