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
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue
{
    public class MultiStrikeScript : ConfigurableSkillScriptBase,
                                      GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                                      DamageAbilityComponent.IDamageComponentOptions
    {
        public bool AnimatePoints { get; init; }
        public Animation? Animation { get; init; }
        public IApplyDamageScript ApplyDamageScript { get; init; }
        public int? BaseDamage { get; init; }
        public ushort? AnimationSpeed { get; init; }
        public BodyAnimation BodyAnimation { get; init; }
        public Stat? DamageStat { get; init; }
        public decimal? DamageStatMultiplier { get; init; }
        public Element? Element { get; init; }
        public bool ExcludeSourcePoint { get; init; }
        public TargetFilter Filter { get; init; }
        public int? ManaCost { get; init; }
        public bool? MoreDmgLowTargetHp { get; init; }
        public bool MustHaveTargets { get; init; }
        public decimal? PctHpDamage { get; init; }
        public decimal PctManaCost { get; init; }
        public int Range { get; init; }
        public AoeShape Shape { get; init; }
        public bool SingleTarget { get; init; }
        public bool ShouldNotBreakHide { get; init; }
        public byte? Sound { get; init; }
        public IScript SourceScript { get; init; }
        private IIntervalTimer StrikeTimer { get; }

        private List<Creature>? Targets { get; set; }

        private ActivationContext? Context { get; set; }

        public MultiStrikeScript(Skill subject)
            : base(subject)
        {
            SourceScript = this;
            ApplyDamageScript = ApplyAttackDamageScript.Create();
            StrikeTimer = new IntervalTimer(TimeSpan.FromMilliseconds(350), false);
        }

        public override void OnUse(ActivationContext context)
        {
            Context = context;
            Targets = context.SourceMap.GetEntitiesWithinRange<Creature>(context.Source, 5)
                             .Where(creature => creature.IsHostileTo(context.Source))
                             .OrderBy(creature => creature.DistanceFrom(context.Source)).Take(5)
                             .ToList();
        }

        public override void Update(TimeSpan delta)
        {
            StrikeTimer.Update(delta);

            if (StrikeTimer.IntervalElapsed)
            {
                if ((Targets == null) || (Context?.Source == null))
                    return;
                
                if (Targets.Count <= 0)
                    return;
                
                var target = Targets.FirstOrDefault();

                if (target != null)
                {
                    var destinationPoint = target.DirectionalOffset(target.Direction.Reverse());

                    if (target.MapInstance.IsWalkable(destinationPoint, CreatureType.Normal) &&
                        !target.MapInstance.IsBlockingReactor(destinationPoint))
                    {
                        Context.Source.WarpTo(destinationPoint);
                        var newDirection = target.DirectionalRelationTo(Context.Source);
                        Context.Source.Turn(newDirection);

                        new ComponentExecutor(Context).WithOptions(this)
                                                      .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                                      ?.Execute<DamageAbilityComponent>();
                    }
                }

                if (target != null)
                    Targets.Remove(target);
            }
        }

        public int SplashChance { get; init; }
        public int SplashDistance { get; init; }
        public TargetFilter SplashFilter { get; init; }
    }
}
