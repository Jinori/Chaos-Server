using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class SpareAStickScript : DialogScriptBase
    {
        public SpareAStickScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.GatheringSticks))
            {
                Subject.Text = "Yeah yeah. I heard ya the first time. Go get the branches.";
            }
        }

        public override void OnDisplayed(Aisling source)
        {
            if (!source.Flags.HasFlag(QuestFlag1.GatheringSticks))
            {
                source.Flags.AddFlag(QuestFlag1.GatheringSticks);
            }
        }
    }
}
