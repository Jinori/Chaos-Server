using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.EffectScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.EffectScripts.Wizard
{
    public class PreventAffliction : EffectBase
    {
        public override byte Icon => 112;
        public override string Name => "preventaffliction";

        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(18);

        public override void OnApplied()
        {
            base.OnApplied();

            if (!Subject.Status.HasFlag(Status.PreventAffliction))
                Subject.Status = Status.PreventAffliction;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your magic has been simplified.");
        }

        public override void OnTerminated()
        {
            if (Subject.Status.HasFlag(Status.PreventAffliction))
                Subject.Status &= ~Status.PreventAffliction;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your magic returns to normal.");
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            if (target.Effects.Contains("preventaffliction"))
            {
                (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This magic has already been simplified.");
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
