using System.Configuration;
using Chaos.Common.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class WizardElementScript : DialogScriptBase
{
    public WizardElementScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        var hasElement = source.Trackers.Flags.TryGetFlag(out WizardElement stage);
        var elementCounter = source.Trackers.Enums.TryGetValue(out WizardElementCounter stage2);

        if (source.UserStatSheet.BaseClass is not BaseClass.Wizard)
            return;

        if (!hasElement)
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "kiril_initial":
                {
                    Subject.Reply(
                        source,
                        "Why are you here? I have nothing to teach someone who cannot dedicate themselves to something. Go find Dar and he can help you find your element.");

                    return;
                }
                case "hadrian_initial":
                {
                    Subject.Reply(
                        source,
                        "What are you doing here? You haven't dedicated yourself to an element? You need to study one first before you ever get another. Go speak to Dar, he is the only one who can help you with your decision.");

                    return;
                }
                case "dar_initial":
                {
                    if (Subject.HasOption("Learn Spells"))
                    {
                        var s = Subject.GetOptionIndex("Learn Spells")!.Value;
                        Subject.Options.RemoveAt(s);
                    }

                    if (source.UserStatSheet.BaseClass is BaseClass.Wizard)
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "wizardElement_initial",
                            OptionText = "Wizard Element"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    }
                }

                    break;

                case "dar_chosefire":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Fire);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "May the fire ignite your soul.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Fire Element",
                            "fireWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;

                case "dar_chosewater":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Water);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Let the water flow within you.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Water Element",
                            "waterWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;

                case "dar_choseearth":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Earth);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Sturdy like a rock, be strong.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Earth Element",
                            "earthWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;

                case "dar_chosewind":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Wind);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Understand restraint, be the leaf.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Wind Element",
                            "windWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;
            }
        
        if (hasElement && !elementCounter)
            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "dar_initial":
                {
                    if (source.UserStatSheet.BaseClass is BaseClass.Wizard && (source.UserStatSheet.Level >= 50))
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "wizardElement_initial2",
                            OptionText = "Second Wizard Element"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    }
                }

                    break;

                case "wizardelement_initial2":
                {
                    if (hasElement && (stage != WizardElement.Fire))
                        Subject.Options.Add(new DialogOption
                        {
                            DialogKey = "dar_firedescription",
                            OptionText = "Fire Element"
                        });
                    
                    if (hasElement && (stage != WizardElement.Water))
                        Subject.Options.Add(new DialogOption
                        {
                            DialogKey = "dar_waterdescription",
                            OptionText = "Water Element"
                        });
                    
                    if (hasElement && (stage != WizardElement.Earth))
                        Subject.Options.Add(new DialogOption
                        {
                            DialogKey = "dar_earthdescription",
                            OptionText = "Earth Element"
                        });
                    
                    if (hasElement && (stage != WizardElement.Wind))
                        Subject.Options.Add(new DialogOption
                        {
                            DialogKey = "dar_winddescription",
                            OptionText = "Wind Element"
                        });

                    break;
                }

                case "dar_chosefire":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Fire);
                    source.Trackers.Enums.Set(WizardElementCounter.Has2ndElement);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "May the fire ignite your soul.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Fire Element",
                            "fireWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;

                case "dar_chosewater":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Water);
                    source.Trackers.Enums.Set(WizardElementCounter.Has2ndElement);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Let the water flow within you.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Water Element",
                            "waterWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;

                case "dar_choseearth":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Earth);
                    source.Trackers.Enums.Set(WizardElementCounter.Has2ndElement);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Sturdy like a rock, be strong.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Earth Element",
                            "earthWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;

                case "dar_chosewind":
                {
                    source.Trackers.Flags.AddFlag(WizardElement.Wind);
                    source.Trackers.Enums.Set(WizardElementCounter.Has2ndElement);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Understand restraint, be the leaf.");

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Studies the Wind Element",
                            "windWizard",
                            MarkIcon.Wizard,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));
                }

                    break;
            }
    }
}