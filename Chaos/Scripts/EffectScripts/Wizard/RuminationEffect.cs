using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.EffectScripts.Wizard
{
    public class RuminationEffect : AnimatingEffectBase
    {
        public override byte Icon => 91;
        public override string Name => "rumination";
        protected override Animation Animation { get; } = new()
        {
            AnimationSpeed = 300,
            TargetAnimation = 58
        };

        protected Point point { get; set; }

        /// <inheritdoc />
        protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
        /// <inheritdoc />
        protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);

        protected override void OnIntervalElapsed()
        {
            //Check if they have moved from the original location
            var currentPoint = new Point(Subject.X, Subject.Y);
            if (!point.Equals(currentPoint))
            {
                Subject.Effects.Terminate(Name);
                return;
            }

            //If their mana bar is full, stop the effect from draining further hp
            if (Subject.StatSheet.CurrentMp >= Subject.StatSheet.EffectiveMaximumMp)
            {
                Subject.Effects.Terminate(Name);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You cannot gain any more mana.");
                return;
            }

            //If they drop under 2% health. Stop the effect.
            if (Subject.StatSheet.HealthPercent < 2)
            {
                Subject.Effects.Terminate(Name);
                return;
            }

            Subject.StatSheet.SubtractHealthPct(5);
            Subject.StatSheet.AddManaPct(2);

            //Show Vitality
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        }

        public override void OnApplied()
        {
            base.OnApplied();

            if (!Subject.Status.HasFlag(Status.Rumination))
                Subject.Status = Status.Rumination;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You sacrifice your health in return for mana gains.");
            point = new Point(Subject.X, Subject.Y);
        }

        public override void OnTerminated()
        {
            if (Subject.Status.HasFlag(Status.Rumination))
                Subject.Status &= ~Status.Rumination;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You stop your mana recovery.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("rumination"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body is already scarificing itself.");
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
