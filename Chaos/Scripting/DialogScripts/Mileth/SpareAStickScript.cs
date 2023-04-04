using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
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
        if (source.Trackers.Flags.HasFlag(QuestFlag1.GatheringSticks))
            Subject.Reply(source, "Yeah yeah. I heard ya the first time. Go get the branches.");
    }
}