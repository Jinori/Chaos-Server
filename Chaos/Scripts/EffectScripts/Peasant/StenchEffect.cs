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
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripts.EffectScripts.Peasant
{
    public class StenchEffect : AnimatingEffectBase
    {
        protected IApplyDamageScript ApplyDamageScript { get; }
        
        public StenchEffect()
        {
            ApplyDamageScript = DefaultApplyDamageScript.Create();
        }

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
                //ApplyDamageScript.ApplyDamage(Subject, target, this, Subject.StatSheet.Level);
                target.ShowHealth();
            }
        }

        public override void OnTerminated()
        {
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A foul stench begins to dissipate.");
        }
    }
}
