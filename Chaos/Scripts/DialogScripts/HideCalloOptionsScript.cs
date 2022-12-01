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
    public class HideCalloOptionsScript : DialogScriptBase
    {
        public HideCalloOptionsScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.SpareAStickComplete))
            {
                if (Subject.GetOptionIndex("Spare A Stick").HasValue)
                {
                    int s = Subject.GetOptionIndex("Spare A Stick")!.Value;
                    Subject.Options.RemoveAt(s);
                }      
            }
        }
    }
}
