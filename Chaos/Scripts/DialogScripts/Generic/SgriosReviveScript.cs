﻿using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Generic
{
    public class SgriosReviveScript : DialogScriptBase
    {
        
        private readonly ISimpleCache SimpleCache;
        
        public SgriosReviveScript(Dialog subject, ISimpleCache simpleCache) : base(subject) => SimpleCache = simpleCache;

        public override void OnDisplayed(Aisling source)
        {
            if (source.IsAlive) return;

            //Change their sprite back to normal
            if (source.Gender is Gender.Male && source.BodySprite is not BodySprite.Male)
                source.BodySprite = BodySprite.Male;
            if (source.Gender is Gender.Female && source.BodySprite is not BodySprite.Female)
                source.BodySprite = BodySprite.Female;

            //They are no longer dead!
            source.IsDead = false;

            //Let's restore their hp/mp to %20
            source?.StatSheet.AddHealthPct(20);
            source?.StatSheet.AddManaPct(20);

            //Refresh the users health bar & turn direction
            source?.Client.SendAttributes(StatUpdateType.Vitality);
            source.Turn(Direction.Down);
            source?.Refresh(true);

            //Let's tell the player they have been revived
            source?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Sgrios mumbles unintelligble gibberish");
            source?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived and sent home.");
            Subject.Close(source);
            
            //Warp them to the Inn
            Point point;
            point = new Point(9, 6);
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_Inn");
            source.TraverseMap(mapInstance, point, true);
        }
    }
}