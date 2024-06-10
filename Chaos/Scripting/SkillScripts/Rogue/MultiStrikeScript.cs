using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue
{
    public class MultiStrikeScript : ConfigurableSkillScriptBase,
                                      GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                                      DamageAbilityComponent.IDamageComponentOptions
    {
        /// <inheritdoc />
        public bool AnimatePoints { get; init; }
        /// <inheritdoc />
        public Animation? Animation { get; init; }

        /// <inheritdoc />
        public IApplyDamageScript ApplyDamageScript { get; init; }
        /// <inheritdoc />
        public int? BaseDamage { get; init; }
        /// <inheritdoc />
        public ushort? AnimationSpeed { get; init; }
        /// <inheritdoc />
        public BodyAnimation BodyAnimation { get; init; }
        /// <inheritdoc />
        public Stat? DamageStat { get; init; }
        /// <inheritdoc />
        public decimal? DamageStatMultiplier { get; init; }
        /// <inheritdoc />
        public Element? Element { get; init; }
        /// <inheritdoc />
        public bool ExcludeSourcePoint { get; init; }
        /// <inheritdoc />
        public TargetFilter Filter { get; init; }
        /// <inheritdoc />
        public int? ManaCost { get; init; }
        /// <inheritdoc />
        public bool? MoreDmgLowTargetHp { get; init; }
        /// <inheritdoc />
        public bool MustHaveTargets { get; init; }
        /// <inheritdoc />
        public decimal? PctHpDamage { get; init; }
        /// <inheritdoc />
        public decimal PctManaCost { get; init; }
        /// <inheritdoc />
        public int Range { get; init; }
        /// <inheritdoc />
        public AoeShape Shape { get; init; }
        /// <inheritdoc />
        public bool SingleTarget { get; init; }
        /// <inheritdoc />
        public bool ShouldNotBreakHide { get; init; }
        /// <inheritdoc />
        public byte? Sound { get; init; }
        /// <inheritdoc />
        public IScript SourceScript { get; init; }

        /// <inheritdoc />
        public MultiStrikeScript(Skill subject)
            : base(subject)
        {
            SourceScript = this;
            ApplyDamageScript = ApplyAttackDamageScript.Create();
        }

        /// <inheritdoc />
        public override async void OnUse(ActivationContext context)
        {
            var targets = context.SourceMap.GetEntitiesWithinRange<Creature>(context.Source, 5)
                                           .Where(creature => creature.IsHostileTo(context.Source))
                                           .OrderBy(creature => creature.DistanceFrom(context.Source))
                                           .ToList();

            // Execute on all targets except the closest one first
            foreach (var target in targets.Skip(1))
                await ExecuteAbilityOnTarget(context, target);

            // Execute on the closest target last
            if (targets.Count != 0)
                await ExecuteAbilityOnTarget(context, targets.First());
        }

        private async Task ExecuteAbilityOnTarget(ActivationContext context, Creature target)
        {
            var behindTargetDirection = target.DirectionalRelationTo(context.SourcePoint);
            var destinationPoint = target.DirectionalOffset(behindTargetDirection);
                
            if (context.SourceMap.IsWalkable(destinationPoint, context.Source.Type) &&
                !context.SourceMap.IsBlockingReactor(destinationPoint))
            {
                context.Source.WarpTo(destinationPoint);
                var newDirection = target.DirectionalRelationTo(context.Source);
                context.Source.Turn(newDirection);

                new ComponentExecutor(context).WithOptions(this)
                                              .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                              ?.Execute<DamageAbilityComponent>();
                
                await Task.Delay(350);
            }
        }

        public int SplashChance { get; init; }
        public int SplashDistance { get; init; }
        public TargetFilter SplashFilter { get; init; }
    }
}
