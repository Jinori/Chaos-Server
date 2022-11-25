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
    public class ArdCradhEffect : EffectBase
    {
        public override byte Icon => 84;
        public override string Name => "ard cradh";

        protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(3);

        public override void OnApplied()
        {
            base.OnApplied();

            var attributes = new Attributes
            {
                Ac = -65,
                MagicResistance = 3,
            };

            AislingSubject?.StatSheet.SubtractMp(500);
            Subject?.StatSheet.SubtractBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've been cursed by cradh! AC and MR lowered!");
        }

        public override void OnTerminated()
        {
            var attributes = new Attributes
            {
                Ac = -65,
                MagicResistance = 3,
            };
            Subject?.StatSheet.AddBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Ard Cradh curse has been lifted.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("ard cradh") || target.Effects.Contains("mor cradh") || target.Effects.Contains("cradh") || target.Effects.Contains("beag cradh"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Another curse has already been applied.");
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
