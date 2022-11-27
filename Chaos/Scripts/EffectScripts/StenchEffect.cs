using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;
using Chaos.Time;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.World.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Scripts.EffectScripts
{
    public class StenchEffect : AnimatingEffectBase
    {
        /// <inheritdoc />
        public override byte Icon { get; } = 176;
        /// <inheritdoc />
        public override string Name { get; } = "Stench";

        /// <inheritdoc />
        protected override Animation Animation { get; } = new()
        {
            AnimationSpeed = 100,
            TargetAnimation = 164
        };
        /// <inheritdoc />
        protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
        /// <inheritdoc />
        protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(1);
        /// <inheritdoc />
        protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(400));

        /// <inheritdoc />
        protected override void OnIntervalElapsed()
        {
            if (Subject.StatSheet.ManaPercent < 1)
            {
                Subject.Effects.Terminate(Name);
                return;
            }

            Subject.StatSheet.SubtractManaPct(1);
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
  
            var points = AoeShape.AllAround.ResolvePoints(Subject);
            var targets = Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>()).WithFilter(Subject, TargetFilter.HostileOnly);

            foreach (var target in targets)
            {
                var TargetAisling = target as Aisling;
                target.ApplyDamage(Subject, 1, null);
                TargetAisling?.Client.SendAttributes(StatUpdateType.Vitality);
                target.ShowHealth();
            }
        }
    }
}
