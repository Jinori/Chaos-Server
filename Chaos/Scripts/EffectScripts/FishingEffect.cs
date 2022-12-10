using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;
using Chaos.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Objects.World;
using Chaos.Containers;
using Chaos.Common.Utilities;
using NLog.Targets;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Factories;
using Chaos.Objects.Menu;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;

namespace Chaos.Scripts.EffectScripts
{
    public class FishingEffect : AnimatingEffectBase
    {

        public FishingEffect(IItemFactory itemFactory)
        {
            ItemFactory = itemFactory;
        }

        private readonly IItemFactory ItemFactory;

        /// <inheritdoc />
        public override byte Icon { get; } = 203;
        /// <inheritdoc />
        public override string Name { get; } = "Fishing";

        /// <inheritdoc />
        protected override Animation Animation { get; } = new()
        {
            AnimationSpeed = 100,
            TargetAnimation = 169
        };
        /// <inheritdoc />
        protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
        /// <inheritdoc />
        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(18);
        /// <inheritdoc />
        protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(5));
        List<string> sayings = new List<string>() { "Your bobber slightly dips but nothing happens.", "You feel a bite at the line.", "*yawn* The water is calm and serene.", "Cursing at the sky, you say you'll never give up!", "A small patch of water ripples." };

        /// <inheritdoc />
        protected override void OnIntervalElapsed()
        {
            var FishingSpots = Subject.MapInstance.GetEntities<ReactorTile>().Where(x => x.ScriptKeys.Contains("FishingSpot") && x.X.Equals(Subject.X) && x.Y.Equals(Subject.Y));
            if (FishingSpots is null)
            {
                Subject.Effects.Terminate("Fishing");
                return;
            }

            int chance = Randomizer.RollRange(5000, 99, RandomizationType.Negative);
            //0.3%
            if (chance >= 4985)
            {
                var item = ItemFactory.CreateFaux("giftbox");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a giftbox!");
                return;
            }
            //0.5%
            else if (chance >= 4975 && chance < 4985)
            {
                var item = ItemFactory.CreateFaux("purplewhopper");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Purple Whopper");
                return;
            }
            //.8%
            else if (chance >= 4960 && chance < 4975)
            {
                var item = ItemFactory.CreateFaux("lionfish");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Lion Fish");
                return;
            }
            //1%
            else if (chance >= 4950 && chance < 4960)
            {
                var item = ItemFactory.CreateFaux("rockfish");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Rock Fish");
                return;
            }
            //1.5%
            else if (chance >= 4925 && chance < 4950)
            {
                var item = ItemFactory.CreateFaux("pike");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Pike");
                return;
            }
            //2%
            else if (chance >= 4900 && chance < 4925)
            {
                var item = ItemFactory.CreateFaux("Perch");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Perch");
                return;
            }
            //2.5%
            else if (chance >= 4875 && chance < 4900)
            {
                var item = ItemFactory.CreateFaux("Bass");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Bass");
                return;
            }
            //3%
            else if (chance >= 4850 && chance < 4875)
            {
                var item = ItemFactory.CreateFaux("Bass");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Trout");
                return;
            }
            else if (chance <= 200)
            {
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You recast your fishing rod in frustration.");
                AislingSubject?.AnimateBody(BodyAnimation.Assail);
            }
            else
            {
                var saying = sayings.PickRandom();
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, saying);
            }
        }

        public override void OnTerminated()
        {
            foreach (var reactor in Subject.MapInstance.GetEntities<ReactorTile>().Where(x => x.ScriptKeys.Contains("FishingSpot") && x.X.Equals(Subject.X) && x.Y.Equals(Subject.Y)))
            {
                reactor.OnWalkedOn(Subject);
            }
        }
    }
}
