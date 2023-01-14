using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Data;

namespace Chaos.Scripts.SpellScripts.Healing
{
    internal class GentleTouchScript : BasicSpellScriptBase
    {
        public GentleTouchScript(Spell subject) : base(subject)
        {
        }

        protected virtual void ApplyHealing(SpellContext context, IEnumerable<Creature> targetEntities)
        {
            foreach (var target in targetEntities)
            {
                var heals = CalculateHealing(context, target);
                target.ApplyHealing(target, heals);
            }
        }

        protected virtual int CalculateHealing(SpellContext context, Creature target)
        {
            var heals = BaseHealing ?? 0;

            if (HealStat.HasValue)
            {
                var multiplier = HealStatMultiplier ?? 1;

                heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
            }

            return heals;
        }

        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            if (ManaSpent.HasValue)
            {
                //Require mana
                if (context.Source.StatSheet.CurrentMp < ManaSpent.Value)
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                    return;
                }
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(ManaSpent.Value);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }

            var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
            ApplyHealing(context, targets.TargetEntities);
        }
        
                
        #region ScriptVars
        protected int? BaseHealing { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        protected int? ManaSpent { get; init; }
        #endregion
    }
}
