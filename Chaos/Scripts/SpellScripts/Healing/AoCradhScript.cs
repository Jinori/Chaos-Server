using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Data;

namespace Chaos.Scripts.SpellScripts.Healing
{
    public class AoCradhScript : BasicSpellScriptBase
    {
        public AoCradhScript(Spell subject) : base(subject)
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

            if (CradhNameToRemove is not null)
            {
                var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
                foreach (var target in targets.TargetEntities)
                {
                    target.Effects.Dispel(CradhNameToRemove);
                    context.TargetAisling?.Client.SendAttributes(StatUpdateType.Full);
                }
            }
        }
        
        #region ScriptVars
        protected int? ManaSpent { get; init; }
        protected string? CradhNameToRemove { get; init; }
        #endregion
    }
}
