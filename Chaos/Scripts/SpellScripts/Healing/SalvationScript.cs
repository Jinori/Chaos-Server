using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Data;

namespace Chaos.Scripts.SpellScripts.Healing
{
    internal class SalvationScript : BasicSpellScriptBase
    {

        public SalvationScript(Spell subject) : base(subject)
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
            var heals = (int)target.StatSheet.EffectiveMaximumHp;
            return heals;
        }



        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            //Require mana
            if (context.Source.StatSheet.CurrentMp < 1)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                return;
            }
            if (context.SourceAisling?.StatSheet.CurrentMp >= 1)
            {
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(context.Source.StatSheet.MaximumMp);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }
            
            var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
            ApplyHealing(context, targets.TargetEntities);
        }
    }
}