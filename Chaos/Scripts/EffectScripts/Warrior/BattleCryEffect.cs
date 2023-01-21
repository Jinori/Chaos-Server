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

namespace Chaos.Scripts.EffectScripts.Warrior
{
    public class BattleCryEffect : EffectBase
    {
        public override byte Icon => 134;
        public override string Name => "battlecry";

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(18);

        public override void OnApplied()
        {
            base.OnApplied();

            if (!Subject.Status.HasFlag(Status.BattleCry))
                Subject.Status = Status.BattleCry;
            AislingSubject?.StatSheet.SubtractMp(100);
            var attributes = new Attributes
            {
                AtkSpeedPct = 5,
            };
            AislingSubject?.StatSheet.AddBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your feel your body accelerate faster.");
        }

        public override void OnTerminated()
        {
            if (Subject.Status.HasFlag(Status.BattleCry))
                Subject.Status &= ~Status.BattleCry;
            var attributes = new Attributes
            {
                AtkSpeedPct = 5,
            };
            AislingSubject?.StatSheet.SubtractBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body has returned to normal.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (source.StatSheet.CurrentMp < 100)
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana.");
                return false;
            }
            if (target.Effects.Contains("battlecry"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your muscles are at their maximum speed.");
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