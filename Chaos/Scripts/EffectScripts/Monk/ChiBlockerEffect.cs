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
using Chaos.Definitions;

namespace Chaos.Scripts.EffectScripts.Monk
{
    public class ChiBlockerEffect : EffectBase
    {
        public override byte Icon => 171;
        public override string Name => "chiBlocker";
        protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(25);

        public override void OnApplied()
        {
            base.OnApplied();

            if (!Subject.Status.HasFlag(Status.ChiBlocker))
                Subject.Status = Status.ChiBlocker;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        }

        public override void OnTerminated()
        {
            if (Subject.Status.HasFlag(Status.ChiBlocker))
                Subject.Status &= ~Status.ChiBlocker;
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        }
    }
}
