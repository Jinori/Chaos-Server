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
        
        public SgriosReviveScript(Dialog subject, ISimpleCache simpleCache) : base(subject)
        {
            SimpleCache = simpleCache;
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.IsAlive) return;

            source.BodySprite = source.Gender switch
            {
                Gender.Male when source.BodySprite is not BodySprite.Male => BodySprite.Male,
                Gender.Female when source.BodySprite is not BodySprite.Female => BodySprite.Female,
                _ => source.BodySprite
            };

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
            
            Subject.Close(source!);

            MapInstance mapInstance;
            Point point;
            point = new Point(6, 6);
            mapInstance = SimpleCache.Get<MapInstance>("mileth_Inn");
            source?.TraverseMap(mapInstance, point, true);
        }
    }
}
