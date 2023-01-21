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
using Chaos.Definitions;

namespace Chaos.Scripts.EffectScripts.Rogue
{
    public class DetectTrapsEffect : EffectBase
    {
        public override byte Icon => 105;
        public override string Name => "detectTraps";

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(200);

        public override void OnApplied()
        {
            base.OnApplied();

            if (!Subject.Status.HasFlag(Status.DetectTraps))
                Subject.Status = Status.DetectTraps;
            
            AislingSubject?.StatSheet.SubtractMp(100);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your sense of survival peaks.");
        }

        public override void OnTerminated()
        {
            if (Subject.Status.HasFlag(Status.DetectTraps))
                Subject.Status &= ~Status.DetectTraps;
            
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your senses have returned to normal.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (source.StatSheet.CurrentMp < 100)
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana.");
                return false;
            }
            if (target.Effects.Contains("detectTraps"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've currently peaked your senses.");
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