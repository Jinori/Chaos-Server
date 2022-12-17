using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts.Mileth
{
    public class ArmsLoreGiveExpScript : DialogScriptBase
    {
        public ArmsLoreGiveExpScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.Arms))
            {
                return;
            }

            source.Flags.AddFlag(QuestFlag1.Arms);
            source.GiveExp(1500);
        }
    }
}
