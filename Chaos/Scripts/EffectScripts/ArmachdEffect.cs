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
    public class ArmachdEffect : EffectBase
    {
        public override byte Icon => 94;
        public override string Name => "armachd";

        protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(5);

        public override void OnApplied(Creature target)
        {
            base.OnApplied(target);

            var attributes = new Attributes
            {
                Ac = -15,
            };

            AislingSubject?.StatSheet.SubtractMp(100);
            Subject?.StatSheet.SubtractBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor increased.");
        }

        public override void OnTerminated()
        {
            var attributes = new Attributes
            {
                Ac = -15,
            };
            Subject?.StatSheet.AddBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor has returned to normal.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("armachd"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor has already been applied.");
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
