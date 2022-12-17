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

namespace Chaos.Scripts.EffectScripts.Wizard
{
    public class BeagCradhEffect : EffectBase
    {
        public override byte Icon => 5;
        public override string Name => "beag cradh";

        protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(5);

        public override void OnApplied()
        {
            base.OnApplied();

            var attributes = new Attributes
            {
                Ac = -20,
            };

            Subject?.StatSheet.SubtractBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've been cursed by beag cradh! AC lowered!");
        }

        public override void OnTerminated()
        {
            var attributes = new Attributes
            {
                Ac = -20,
            };
            Subject?.StatSheet.AddBonus(attributes);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "beag cradh curse has been lifted.");
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