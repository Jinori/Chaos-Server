using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class IsabelleMantisDeadScript : DialogScriptBase
    {
        public IsabelleMantisDeadScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.IsabelleQuest))
            {
                source.Flags.RemoveFlag(QuestFlag1.IsabelleQuest);
                source.Flags.AddFlag(QuestFlag1.IsabelleMantisDead);
            }
        }
    }
}
