using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.EffectScripts
{
    public class ClawFistEffect : AnimatingEffectBase
    {
        public override byte Icon => 101;
        public override string Name => "clawfist";
        protected override Animation Animation { get; } = new()
        {
            AnimationSpeed = 300,
            TargetAnimation = 54
        };
        /// <inheritdoc />
        protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
        /// <inheritdoc />
        protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(400));

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(10);

        protected override void OnIntervalElapsed()
        {
        }

        public override void OnApplied()
        {
            base.OnApplied();

            if (!Subject.Status.HasFlag(Status.ClawFist))
                Subject.Status = Status.ClawFist;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your first rush with a burst of energy.");
        }

        public override void OnTerminated()
        {
            if (Subject.Status.HasFlag(Status.ClawFist))
                Subject.Status &= ~Status.ClawFist;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your hands return to normal.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("clawfist"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");
                return false;
            }
            else
                return true;
        }

        public override void OnDispelled()
        {
            OnTerminated();
        }
    }
}