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
        : base(subject)
    {
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasElement = source.Trackers.Flags.TryGetFlag(out WizardElement stage);

        if (!hasElement)
            switch (Subject.Template.TemplateKey.ToLower())
            {
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
    }

}