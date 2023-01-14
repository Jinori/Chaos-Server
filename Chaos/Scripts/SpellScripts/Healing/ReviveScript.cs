using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Healing
{
    public class SelfReviveScript : ReviveScript
    {
        public SelfReviveScript(Spell subject) : base(subject)
        {
        }
        
        public override bool CanUse(SpellContext context)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can only cast this spell when you are dead.");
            return context.Source.Equals(context.Target) && !context.Source.IsAlive;
        }
    }

    public class ReviveScript : BasicSpellScriptBase
    {
        public override bool CanUse(SpellContext context)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You cannot use this spell while dead.");
            return context.Source.IsAlive;
        }

        public ReviveScript(Spell subject) : base(subject)
        {
        }
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
            foreach (var target in targets.TargetEntities)
            {
                if (!target.IsAlive)
                {

                    //Let's restore their hp/mp to %20
                    target.StatSheet.AddHealthPct(20);
                    target.StatSheet.AddManaPct(20);

                    //Refresh the users health bar
                    context.TargetAisling?.Client.SendAttributes(StatUpdateType.Vitality);

                    //Let's tell the player they have been revived
                    context.TargetAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived.");
                }
                else
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This target isn't dead.");
                    return;
                }
            }
        }
        
        #region ScriptVars
        protected int? ManaSpent { get; init; }
        #endregion
    }
}
