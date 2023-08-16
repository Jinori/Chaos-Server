using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class SpareAStickScript : DialogScriptBase
{
    public SpareAStickScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplayed(Aisling source)
    {
        if (!source.Trackers.Flags.HasFlag(QuestFlag1.GatheringSticks))
            source.Trackers.Flags.AddFlag(QuestFlag1.GatheringSticks);
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "callo_initial":
                if (source.UserStatSheet.Level < 11)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "callo_spareastickinitial",
                    OptionText = "Spare a Stick"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
        }

        if (source.Trackers.Flags.HasFlag(QuestFlag1.GatheringSticks))
            Subject.Reply(source, "Yeah yeah. I heard ya the first time. Go get the branches.");
    }
}