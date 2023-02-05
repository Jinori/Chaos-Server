using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
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

    public class ReviveScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
    {
        public override bool CanUse(SpellContext context)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You cannot use this spell while dead.");
            return context.Source.IsAlive;
        }

        protected ManaCostComponent ManaCostComponent { get; }
        
        public ReviveScript(Spell subject) : base(subject) => ManaCostComponent = new ManaCostComponent();

        public override void OnUse(SpellContext context)
        {
            ManaCostComponent.ApplyManaCost(context, this);

            var targets = AbilityComponent.Activate<Creature>(context, this);
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
        public int? ManaCost { get; init; }
        public decimal PctManaCost { get; init; }
        #endregion
    }
}
