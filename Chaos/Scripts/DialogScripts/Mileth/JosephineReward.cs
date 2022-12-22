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
    public class JosephineRewardScript : DialogScriptBase
    {
        public JosephineRewardScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop))
            {
                Subject.Text = "Riona sent you? I do have her dye, I'll let her know! Are you interested in a hair style?";
                source.Flags.RemoveFlag(QuestFlag1.HeadedToBeautyShop);
                source.Flags.AddFlag(QuestFlag1.TalkedToJosephine);
                source.GiveExp(1000);
                source.TryGiveGold(1000);
            }
        }
    }
}
