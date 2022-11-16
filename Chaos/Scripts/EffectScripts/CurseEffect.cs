using Chaos.Common.Definitions;
using Chaos.Objects.World;
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
    public class CurseEffect : IntervalEffectBase
    {
        public override byte Icon => 20;
        public override string Name => "Curse";
        protected int? curseAC { get; init; }

    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMinutes(2));

        protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(2);

        protected override void OnIntervalElapsed()
        {
            if (curseAC.HasValue)
            {
                //
            }
            AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        }
    }
}
