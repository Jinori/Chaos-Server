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
    public class IsabelleQuestScript : DialogScriptBase
    {
        public IsabelleQuestScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplayed(Aisling source)
        {
            if (!source.Flags.HasFlag(QuestFlag1.IsabelleQuest))
            {
                source.Flags.AddFlag(QuestFlag1.IsabelleQuest);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Isabelle seemed really frightened, go to the peak.");
            }
        }
    }
}
