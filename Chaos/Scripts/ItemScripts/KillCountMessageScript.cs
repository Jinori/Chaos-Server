using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.ItemScripts
{
    public class KillCountMessageScript : ConfigurableItemScriptBase
    {

        protected bool Admin { get; init; }
        
        public KillCountMessageScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (!Admin)
            {
                var killCounts = string.Join(Environment.NewLine, source.killedMonsters.Select(x => string.Join(" - ", x.Key, x.Value)));
                if (killCounts.Length >= 1)
                    source.Client.SendServerMessage(ServerMessageType.WoodenBoard, killCounts);
                else
                    source.Client.SendServerMessage(ServerMessageType.WoodenBoard, "No currently recorded kills.");   
            }
        }
    }
}
