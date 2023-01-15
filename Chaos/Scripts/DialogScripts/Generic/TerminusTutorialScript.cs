using System.Diagnostics.Eventing.Reader;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.DialogScripts;

public class TerminusTutorialScript : DialogScriptBase
{
    public TerminusTutorialScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Enums.TryGetValue(out TutorialQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "terminus_initial":
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "TerminusDeathExplanation",
                        OptionText = "I'm dead?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;
                
            case "terminusgotosgrios":
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                        
                }

                break;
        }
    }
}