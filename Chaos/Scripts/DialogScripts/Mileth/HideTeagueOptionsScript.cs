using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts.Mileth;

internal class HideTeagueOptionsScript : DialogScriptBase
{
    public HideTeagueOptionsScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt))
        {
            Subject.Text = "Please end my terrors... good luck on your quest.";
            Subject.Type = MenuOrDialogType.Normal;

            return;
        }

        if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.TerrorOfTheCrypt, out var timedEvent))
        {

            Subject.Text = $"Thank you for ending my terrors Aisling, I hope I sleep better tonight. Come see me later (({
                timedEvent.Remaining.ToReadableString()}))";
            Subject.Type = MenuOrDialogType.Normal;
        }
    }
}