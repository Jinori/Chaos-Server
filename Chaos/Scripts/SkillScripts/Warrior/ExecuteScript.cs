using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Data;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripts.SkillScripts.Warrior
{
    public class ExecuteScript : BasicSkillScriptBase
    {
        protected IApplyDamageScript ApplyDamageScript { get; }
        protected DamageComponent DamageComponent { get; }
        protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

        /// <inheritdoc />
        public ExecuteScript(Skill subject)
            : base(subject)
        {
            ApplyDamageScript = ExecuteApplyDamageScript.Create();
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
            var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
            DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
        }

        #region ScriptVars
        protected int? BaseDamage { get; init; }
        protected Stat? DamageStat { get; init; }
        protected decimal? DamageMultiplier { get; init; }
        #endregion
    }
}

