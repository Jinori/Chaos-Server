using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

internal class HideTeagueOptionsScript : DialogScriptBase
{
    public HideTeagueOptionsScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Trackers.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt))
        {
            Subject.Text = "Please end my terrors... good luck on your quest.";
            Subject.Type = MenuOrDialogType.Normal;

            return;
        }

        if (source.Trackers.TimedEvents.TryConsumeEvent("TerrorOfTheCrypt", out var timedEvent))
        {

            Subject.Text = $"Thank you for ending my terrors Aisling, I hope I sleep better tonight. Come see me later (({
                timedEvent.Remaining.ToReadableString()}))";
            Subject.Type = MenuOrDialogType.Normal;
        }
    }
}