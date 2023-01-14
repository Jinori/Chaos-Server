using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Debuffs
{
    public class GoadScript : BasicSpellScriptBase
    {

        public GoadScript(Spell subject) : base(subject)
        {
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
            foreach (var target in targets.TargetEntities)
            {
                var monster = target as Monster;
                monster?.AggroList.AddOrUpdate(context.Source.Id, _ => 1000, (_, currentAggro) => currentAggro + 1000);
            }

            context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
        }
        
        #region ScriptVars
        protected int? ManaSpent { get; init; }
        #endregion
    }
}