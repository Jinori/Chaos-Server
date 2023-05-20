using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
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
            Subject.Reply(source, "Please end my terrors... good luck on your quest.");

            return;
        }

        if (source.Trackers.TimedEvents.HasActiveEvent("TerrorOfTheCrypt", out var timedEvent))
        {

            Subject.Reply(
                source,
                $"Thank you for ending my terrors Aisling, I hope I sleep better tonight. Come see me later (({
                    timedEvent.Remaining.ToReadableString()}))");
        }
    }
}