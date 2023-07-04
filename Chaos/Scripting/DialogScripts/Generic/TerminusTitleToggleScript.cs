using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusTitleToggleScript : DialogScriptBase
{
    public TerminusTitleToggleScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out TutorialQuestStage stage);

        if (stage != TutorialQuestStage.CompletedTutorial)
            return;

        if (source.Titles.Count < 2)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "terminus_titleChange",
                    OptionText = "Title Change"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }

                var first = source.Titles.First();
                source.Titles.Remove(first);
                source.Titles.Add(first);
                source.Client.SendSelfProfile();

                break;
        }
    }
}