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
    public class HideRionaOptionsScript : DialogScriptBase
    {
        public HideRionaOptionsScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop) || source.Flags.HasFlag(QuestFlag1.TalkedToJosephine))
            {
                //Remove Beauty Shop from being visible
                Subject.Options.RemoveAt(2);
            }
        }
    }
}
