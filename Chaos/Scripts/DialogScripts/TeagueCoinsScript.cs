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
    public class TeagueCoinsScript : DialogScriptBase
    {
        public TeagueCoinsScript(Dialog subject) : base(subject)
        {

        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Gold >= 1000)
            {
                source.TryTakeGold(1000);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The old man's eyes light up with glee..");
            }
            if (source.Gold < 1000)
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You reach into your coin purse but it's not that full..");
                Subject.Text = "You almost had me excited there.. don't promise what you can't make true.";
                Subject.Type = MenuOrDialogType.Normal;
            }
        }
    }
}
