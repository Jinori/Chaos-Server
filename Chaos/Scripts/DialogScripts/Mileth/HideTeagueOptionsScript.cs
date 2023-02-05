using Chaos.Common.Definitions;
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
        }
    }
}