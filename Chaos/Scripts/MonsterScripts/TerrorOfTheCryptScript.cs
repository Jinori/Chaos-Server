using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.MonsterScripts
{
    public class TerrorOfTheCryptScript : MonsterScriptBase
    {
        private readonly ISimpleCache SimpleCache;


        public TerrorOfTheCryptScript(Monster subject, ISimpleCache simpleCache) : base(subject)
        {
            SimpleCache= simpleCache;
        }

        public override void OnDeath()
        {
            var mapInstance = SimpleCache.Get<MapInstance>("cryptTerrorReward");
            foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>())
            {
                aisling.TraverseMap(mapInstance, new Point(4, 5));
                aisling.Flags.RemoveFlag(QuestFlag1.TerrorOfCryptHunt);
                aisling.Flags.AddFlag(QuestFlag1.TerrorOfCryptComplete);
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The terror will no longer make the Old Man suffer.");
            }
        }
    }
}
