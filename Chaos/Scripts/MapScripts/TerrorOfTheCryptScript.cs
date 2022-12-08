using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.MapScripts
{
    public class TerrorOfTheCryptScript : MapScriptBase
    {
        private readonly IMonsterFactory MonsterFactory;

        public TerrorOfTheCryptScript(MapInstance subject, IMonsterFactory monsterFactory) : base(subject)
        {
            MonsterFactory = monsterFactory;
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling)
                return;

            var aisling = creature as Aisling;

            aisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You hear a creak, as the coffin begins to slide open...");
            Spawn(aisling!);
        }

        public void Spawn(Aisling aisling)
        {
            var group = aisling.Group?.Where(x => x.WithinRange(new Point(8, 8)));
            if (group is null)
                return;

            int groupCount = 0;
            foreach (var member in group)
            {
                if (member.WithinLevelRange(aisling))
                    ++groupCount;
            }
            if (groupCount.Equals(group.Count()))
            {
                var fire = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 13
                };

                _ = Task.Run(
               async () =>
               {
                   try
                   {
                       var shape = new Rectangle(new Point(8, 8), 5, 5);
                       var outline = shape.GetOutline().ToList();
                       var reverseOutline = outline.AsEnumerable().Reverse().ToList();

                       for (var i = 0; i < outline.Count; i++)
                       {
                           var point1 = outline[i];
                           var point2 = reverseOutline[i];
                           await using (_ = await aisling!.MapInstance.Sync.WaitAsync())
                           {
                               Subject.ShowAnimation(fire.GetPointAnimation(point1));
                               Subject.ShowAnimation(fire.GetPointAnimation(point2));
                           }
                           await Task.Delay(200);
                       }

                       if (aisling!.StatSheet.Level <= 11)
                       {
                           var monster = MonsterFactory.Create("terrorLowInsight", Subject, new Point(4, 8));
                           await using (_ = await aisling!.MapInstance.Sync.WaitAsync())
                           {
                               if (Subject.GetEntities<Creature>().Where(x => x.Name.Equals("Terror Of The Crypt")).Count() < 1)
                               {
                                   Subject.AddObject(monster, new Point(8, 8));
                               }
                           }
                       }
                   }
                   catch
                   {
                       //ignored
                   }
               });
            }
        }
    }
}
