using Chaos.Common.Utilities;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Data;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripts.SkillScripts.Warrior
{
    public class ExecuteScript : BasicSkillScriptBase
    {
        protected IApplyDamageScript ApplyDamageScript { get; }

        /// <inheritdoc />
        public ExecuteScript(Skill subject)
            : base(subject)
        {
            ApplyDamageScript = DefaultApplyDamageScript.Create();
        }

        /// <inheritdoc />
        public override void OnUse(ActivationContext context)
        {
            var targets = AbilityComponent.Activate<Creature>(context, this);
            foreach (var target in targets.TargetEntities)
            {
                if (target.StatSheet.HealthPercent <= KillAtHealthPct)
                {
                    target.StatSheet.SetHp(0);
                    ApplyDamageScript.ApplyDamage(context.Source, target, this, 9999);
                    var healAmount = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, HealAmountPct);
                    context.Source.ApplyHealing(context.Source, healAmount);
                    Subject.Elapsed = Subject.Cooldown / 2;
                }
                else
                {
                    var tenPercent = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, DmgHealthPct);
                    ApplyDamageScript.ApplyDamage(context.Source, target, this, tenPercent);
                }
            }
        }

        #region ScriptVars
        protected int DmgHealthPct { get; init; }
        protected int HealAmountPct { get; init; }
        protected int KillAtHealthPct { get; init; }
        #endregion
    }
}

