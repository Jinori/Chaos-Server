using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Debuffs
{
    public class GoadScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
    {
        protected ManaCostComponent ManaCostComponent { get; }
        public GoadScript(Spell subject) : base(subject) => ManaCostComponent = new ManaCostComponent();

        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            ManaCostComponent.ApplyManaCost(context, this);
            
            var targets = AbilityComponent.Activate<Creature>(context, this);
            foreach (var target in targets.TargetEntities)
            {
                var monster = target as Monster;
                monster?.AggroList.AddOrUpdate(context.Source.Id, _ => 1000, (_, currentAggro) => currentAggro + 1000);
            }

            context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
        }
        
        #region ScriptVars
        public int? ManaCost { get; init; }
        public decimal PctManaCost { get; init; }
        #endregion
    }
}