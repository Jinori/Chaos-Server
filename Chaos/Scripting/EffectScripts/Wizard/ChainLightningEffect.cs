using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard
{
    public sealed class ChainLightningEffect : ContinuousAnimationEffectBase
    {
        /// <inheritdoc />
        protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);

        protected IApplyDamageScript ApplyDamageScript { get; }

        private Creature SourceOfEffect { get; set; } = null!;
        private Creature Target { get; set; } = null!;

        private int Number;

        private readonly IEffectFactory EffectFactory;

        /// <inheritdoc />
        protected override Animation Animation { get; } = new()
        {
            AnimationSpeed = 100,
            TargetAnimation = 242
        };

        /// <inheritdoc />
        protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromHours(1000), false);
        /// <inheritdoc />
        protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);
        
        /// <inheritdoc />
        public override byte Icon => 112;

        private const int MAX_BOUNCES = 5;
        private HashSet<uint> HitTargetIds = new(); // Using HashSet<uint> to track target IDs

        public ChainLightningEffect(IEffectFactory effectFactory)
        {
            EffectFactory = effectFactory;
            ApplyDamageScript = ApplyAttackDamageScript.Create();
        }

        /// <inheritdoc />
        public override string Name => "ChainLightning";

        public override void OnApplied()
        {
        }

        /// <inheritdoc />
        protected override void OnIntervalElapsed()
        {
            BounceToNextTarget(SourceOfEffect, Target);
        }

        public override void OnTerminated()
        {
            // Cleanup logic if needed
        }

        public override bool ShouldApply(Creature source, Creature target)
        {
            SourceOfEffect = source;
            Target = target;

            if (HitTargetIds.Count >= MAX_BOUNCES)
                return false;

            if (!target.IsHostileTo(source))
                return false;

            if (target.Effects.Contains("ChainLightning"))
                return false;

            if (HitTargetIds.Contains(target.Id))
                return false;

            return true;
        }

        private void BounceToNextTarget(Creature source, Creature target)
        {
            // Add the target's ID to the HitTargets set
            HitTargetIds.Add(target.Id);
    
            // Base damage calculation
            var baseDamage = SourceOfEffect.StatSheet.Int * 24 + 800;

            // Calculate reduction based on the number of hits
            var numberOfHits = HitTargetIds.Count;
            var reductionPercentage = Math.Min(0.90, 0.10 * (numberOfHits - 1)); // 10% reduction per hit, max 90%
            var finalDamage = baseDamage * (1 - reductionPercentage); // Apply the reduction

            // Apply the reduced damage
            ApplyDamageScript.ApplyDamage(
                SourceOfEffect,
                target,
                this,
                (int)finalDamage,
                Element.Wind);

            target.ShowHealth();

            // Find nearby targets for the next bounce
            var nearbyTargets = target.MapInstance.GetEntitiesWithinRange<Creature>(target, 2)
                .OrderBy(t => t.DistanceFrom(target))
                .Where(t => !HitTargetIds.Contains(t.Id) && ShouldApply(SourceOfEffect, t))
                .ToList();

            if (nearbyTargets.Any())
            {
                var nextTarget = nearbyTargets.First();
                var effect = EffectFactory.Create("chainlightning");

                // Assuming that your effect has a property or method to set the number of bounces or pass additional data
                if (effect is ChainLightningEffect chainLightningEffect)
                {
                    chainLightningEffect.HitTargetIds = new HashSet<uint>(HitTargetIds);
                }

                nextTarget.Effects.Apply(SourceOfEffect, effect);
                nextTarget.Animate(Animation);
            }
    
            // Terminate the effect after bouncing
            target.Effects.Terminate("ChainLightning");
        }


    }
}
