using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Healing
{
    public class AoPoisonScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
    {
        protected ManaCostComponent ManaCostComponent { get; }
        public AoPoisonScript(Spell subject) : base(subject) => ManaCostComponent = new ManaCostComponent();

        public override void OnUse(SpellContext context)
        {
            ManaCostComponent.ApplyManaCost(context, this);
            var targets = AbilityComponent.Activate<Creature>(context, this);
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
        protected string? CradhNameToRemove { get; init; }
        public int? ManaCost { get; init; }
        public decimal PctManaCost { get; init; }
        #endregion
    }
}
