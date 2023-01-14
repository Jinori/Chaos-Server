using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripts.SkillScripts.Warrior
{
    public class ChargeScript : BasicSkillScriptBase
    {
        protected IApplyDamageScript ApplyDamageScript { get; }
        protected DamageComponent DamageComponent { get; }
        protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

        /// <inheritdoc />
        public ChargeScript(Skill subject)
            : base(subject)
        {
            ApplyDamageScript = DefaultApplyDamageScript.Create();
            DamageComponent = new DamageComponent();

            DamageComponentOptions = new DamageComponent.DamageComponentOptions
            {
                ApplyDamageScript = ApplyDamageScript,
                SourceScript = this,
                BaseDamage = BaseDamage,
                DamageMultiplier = DamageMultiplier,
                DamageStat = DamageStat
            };
        }

        /// <inheritdoc />
        public override void OnUse(ActivationContext context)
        {
            //get the 5 points in front of the source
            var points = AoeShape.Front.ResolvePoints(
                context.SourcePoint,
                5,
                context.Source.Direction,
                context.Map.Template.Bounds);

           var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
           var targetCreature = targets.TargetEntities.FirstOrDefault();

            if (targetCreature is null)
            {
                //if that point is not walkable, continue
                if (!context.Map.IsWalkable(points.Last(), context.Source.Type))
                    return;

                //if it is walkable, warp to that point
                context.Source.WarpTo(points.Last());
                DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
            }

            return;
        }

        #region ScriptVars
        protected int? BaseDamage { get; init; }
        protected Stat? DamageStat { get; init; }
        protected decimal? DamageMultiplier { get; init; }
        #endregion
    }
}
