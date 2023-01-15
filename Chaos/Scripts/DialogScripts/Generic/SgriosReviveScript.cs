using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Generic
{
    public class SgriosReviveScript : DialogScriptBase
    {
        
        private readonly ISimpleCache SimpleCache;
        
        public SgriosReviveScript(Dialog subject) : base(subject)
        {
            
        }

        public override void OnDisplayed(Aisling source)
        {
            if (!source.IsAlive)
            {
                if (source.Gender is Gender.Male && source.BodySprite is not BodySprite.Male)
                    source.BodySprite = BodySprite.Male;
                if (source.Gender is Gender.Female && source.BodySprite is not BodySprite.Female)
                    source.BodySprite = BodySprite.Female;

                //They are no longer dead!
                source.IsDead = false;

                //Let's restore their hp/mp to %20
                source?.StatSheet.AddHealthPct(20);
                source?.StatSheet.AddManaPct(20);

                //Refresh the users health bar
                source?.Client.SendAttributes(StatUpdateType.Vitality);
                source?.Refresh(true);

                //Let's tell the player they have been revived
                source?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived.");
                
                MapInstance mapInstance;
                Point point;
                point = new Point(6, 6);
                mapInstance = SimpleCache.Get<MapInstance>("mileth_Inn");
                source.TraverseMap(mapInstance, point, true);
            }
        }
    }
}
