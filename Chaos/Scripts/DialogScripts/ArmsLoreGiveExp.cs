using Chaos.Common.Definitions;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Legend;
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
