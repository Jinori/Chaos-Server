using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Healing
{
    public class AoPoisonScript : BasicSpellScriptBase
    {
        public AoPoisonScript(Spell subject) : base(subject)
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
                if (target.Effects.Contains("poison"))
                {
                    target.Effects.Dispel("poison");
                    context.TargetAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{context.Source.Name} has healed your poison. You feel fine now.");
                    context.TargetAisling?.Client.SendAttributes(StatUpdateType.Full);
                }
                else
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This target isn't affected by poison.");
                    return;
                }   
            }
        }
                
        #region ScriptVars
        protected int? ManaSpent { get; init; }
        protected string? CradhNameToRemove { get; init; }
        #endregion
    }
}
