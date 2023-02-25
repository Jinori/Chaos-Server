using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class HideCalloOptionsScript : DialogScriptBase
{
    public HideCalloOptionsScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.SpareAStickComplete))
            if (Subject.GetOptionIndex("Spare A Stick").HasValue)
            {
                var s = Subject.GetOptionIndex("Spare A Stick")!.Value;
                Subject.Options.RemoveAt(s);
            }
    }
}