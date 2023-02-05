using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Scripts.Components;

namespace Chaos.Scripts.SpellScripts.Healing
{
    internal class BasicGroupHealScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
    {
        public BasicGroupHealScript(Spell subject) : base(subject)
        {
            ManaCostComponent = new ManaCostComponent();
            GroupComponent = new GroupComponent();
            GroupComponentOptions = new GroupComponent.GroupComponentOptions
            {
                SourceScript = this,
                BaseHealing = BaseHealing,
                HealStat = HealStat,
                HealStatMultiplier = HealStatMultiplier,
                Shape = Shape,
                Range = Range,
                BodyAnimation = null,
                Animation = Animation,
                Sound = Sound,
                AnimatePoints = AnimatePoints,
                MustHaveTargets = MustHaveTargets,
                IncludeSourcePoint = IncludeSourcePoint,
            };
        }
        
        protected ManaCostComponent ManaCostComponent { get; }
        protected GroupComponent GroupComponent{ get; }
        protected GroupComponent.GroupComponentOptions GroupComponentOptions { get; }

        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            if (context.SourceAisling?.Group is null)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are not in a group.");
                return;
            }
            ManaCostComponent.ApplyManaCost(context, this);
            var targets = GroupComponent.Activate<Aisling>(context, GroupComponentOptions);
            GroupComponent.ApplyHealing(context, targets.targetEntities!, GroupComponentOptions);
        }
        
        #region ScriptVars
        protected int? BaseHealing { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        public int? ManaCost { get; init; }
        public decimal PctManaCost { get; init; }
        #endregion
    }
}
