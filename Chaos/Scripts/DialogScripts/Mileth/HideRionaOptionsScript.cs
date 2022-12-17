using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts.Mileth
{
    public class HideRionaOptionsScript : DialogScriptBase
    {
        public HideRionaOptionsScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop) || source.Flags.HasFlag(QuestFlag1.TalkedToJosephine))
            {
                if (Subject.GetOptionIndex("Beauty Shop").HasValue)
                {
                    int s = Subject.GetOptionIndex("Beauty Shop")!.Value;
                    Subject.Options.RemoveAt(s);
                }
            }
        }
    }
}
