using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class MonkFormChange: DialogScriptBase
{
    /// <inheritdoc />
    public MonkFormChange(Dialog subject)
        : base(subject) { }

    private readonly Animation WaterForm = new()
    {
        TargetAnimation = 67,
        AnimationSpeed = 100
    };
    
    private readonly Animation FireForm = new()
    {
        TargetAnimation = 102,
        AnimationSpeed = 100
    };
    
    private readonly Animation WindForm = new()
    {
        TargetAnimation = 156,
        AnimationSpeed = 100
    };
    
    private readonly Animation EarthForm = new()
    {
        TargetAnimation = 87,
        AnimationSpeed = 100
    };

    private readonly List<string> AirSkills = [ "airpunch", "tempestkick"];
    private readonly List<string> AirSpells = [ "thunderstance"];
    
    private readonly List<string> FireSkills = [ "firepunch", "dracotailkick"];
    private readonly List<string> FireSpells = [ "smokestance"];
    
    private readonly List<string> EarthSkills = [ "earthpunch", "seismickick"];
    private readonly List<string> EarthSpells = [ "earthenstance"];
    
    private readonly List<string> WaterSkills = [ "airpunch", "tempestkick"];
    private readonly List<string> WaterSpells = [ "miststance"];
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ironfoot_initial":
            {
                if (!source.Trackers.Enums.TryGetValue(out MonkElementForm _))
                    Subject.Reply(source, "You need to learn a form before I can help you, little rabbit.");

                break;
            }
        }
    }

    public void RemoveOldForm(Aisling source, MonkElementForm oldForm)
    {
        source.Legend.Remove($"{oldForm}Monk", out _);

        foreach (var skill in source.SkillBook)
            switch (oldForm)
            {
                case MonkElementForm.Water:
                    if (WaterSkills.Contains(skill.Template.TemplateKey))
                        source.SkillBook.RemoveByTemplateKey(skill.Template.TemplateKey);
                    break;
                case MonkElementForm.Earth:
                    if (EarthSkills.Contains(skill.Template.TemplateKey))
                        source.SkillBook.RemoveByTemplateKey(skill.Template.TemplateKey);
                    break;
                case MonkElementForm.Air:
                    if (AirSkills.Contains(skill.Template.TemplateKey))
                        source.SkillBook.RemoveByTemplateKey(skill.Template.TemplateKey);
                    break;
                case MonkElementForm.Fire:
                    if (FireSkills.Contains(skill.Template.TemplateKey))
                        source.SkillBook.RemoveByTemplateKey(skill.Template.TemplateKey);
                    break;
            }

        foreach (var spell in source.SpellBook)
            switch (oldForm)
            {
                case MonkElementForm.Water:
                    if (WaterSpells.Contains(spell.Template.TemplateKey))
                        source.SpellBook.RemoveByTemplateKey(spell.Template.TemplateKey);
                    break;
                case MonkElementForm.Earth:
                    if (EarthSpells.Contains(spell.Template.TemplateKey))
                        source.SpellBook.RemoveByTemplateKey(spell.Template.TemplateKey);
                    break;
                case MonkElementForm.Air:
                    if (AirSpells.Contains(spell.Template.TemplateKey))
                        source.SpellBook.RemoveByTemplateKey(spell.Template.TemplateKey);
                    break;
                case MonkElementForm.Fire:
                    if (FireSpells.Contains(spell.Template.TemplateKey))
                        source.SpellBook.RemoveByTemplateKey(spell.Template.TemplateKey);
                    break;
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
        source.SendServerMessage(ServerMessageType.ActiveMessage, $"You have dissolved yourself of {oldForm} and have chose {newForm.Humanize().Titleize()}.");
        source.Trackers.Enums.Set(typeof(MonkElementForm), newForm);

        switch (newForm)
        {
            case MonkElementForm.Water:
                source.Animate(WaterForm);
                break;
            case MonkElementForm.Earth:
                source.Animate(EarthForm);
                break;
            case MonkElementForm.Air:
                source.Animate(WindForm);
                break;
            case MonkElementForm.Fire:
                source.Animate(FireForm);
                break;
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is 1)
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "ironfoot_fireconfirm":
                {
                    if (source.Trackers.Enums.TryGetValue(out MonkElementForm form))
                    {
                        RemoveOldForm(source, form);
                        GiveNewForm(source, form, MonkElementForm.Fire);
                    }

                    break;
                }
                case "ironfoot_waterconfirm":
                {
                    if (source.Trackers.Enums.TryGetValue(out MonkElementForm form))
                    {
                        RemoveOldForm(source, form);
                        GiveNewForm(source, form, MonkElementForm.Water);
                    }

                    break;
                }
                case "ironfoot_earthconfirm":
                {
                    if (source.Trackers.Enums.TryGetValue(out MonkElementForm form))
                    {
                        RemoveOldForm(source, form);
                        GiveNewForm(source, form, MonkElementForm.Earth);
                    }

                    break;
                }
                case "ironfoot_airconfirm":
                {
                    if (source.Trackers.Enums.TryGetValue(out MonkElementForm form))
                    {
                        RemoveOldForm(source, form);
                        GiveNewForm(source, form, MonkElementForm.Air);
                    }

                    break;
                }
            }
    }
}