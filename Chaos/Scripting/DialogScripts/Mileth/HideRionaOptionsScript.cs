using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class HideRionaOptionsScript : DialogScriptBase
{
    public HideRionaOptionsScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop) || source.Flags.HasFlag(QuestFlag1.TalkedToJosephine))
            if (Subject.GetOptionIndex("Beauty Shop").HasValue)
            {
                var s = Subject.GetOptionIndex("Beauty Shop")!.Value;
                Subject.Options.RemoveAt(s);
            }
    }
}