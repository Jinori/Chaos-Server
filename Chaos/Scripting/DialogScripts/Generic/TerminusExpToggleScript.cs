using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusExpToggleScript : DialogScriptBase
{
    public TerminusExpToggleScript(Dialog subject) : base(subject)
    {
    }

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out TutorialQuestStage tutorial);

        if (tutorial != TutorialQuestStage.CompletedTutorial)
            return;
        
        var hasFlag = source.Trackers.Enums.TryGetValue(out GainExp stage);
        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "terminus_initial":
            {

                var option = new DialogOption
                {
                    DialogKey = "terminus_expGain",
                    OptionText = "Toggle Experience Gain"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "terminus_expgainyes":
            {
                switch (stage)
                {
                    case GainExp.No:
                        source.Trackers.Enums.Set(GainExp.Yes);
                        source.SendOrangeBarMessage("You will now gain experience.");
                        break;
                    case GainExp.Yes:
                        source.SendOrangeBarMessage("You were already gaining experience, young one.");
                        break;
                }

                break;
            }
            case "terminus_expgainno":
            {
                switch (stage)
                {
                    case GainExp.Yes:
                        source.Trackers.Enums.Set(GainExp.No);
                        source.SendOrangeBarMessage("You will no longer gain experience.");
                        break;
                    case GainExp.No:
                        source.SendOrangeBarMessage("You currently didn't want experience anyway..");
                        break;
                }
                break;
            }
        }
    }
}