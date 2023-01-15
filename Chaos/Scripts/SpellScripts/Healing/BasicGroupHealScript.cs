using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Scripts.Components;

namespace Chaos.Scripts.SpellScripts.Healing
{
    internal class BasicGroupHealScript : BasicSpellScriptBase
    {
        public BasicGroupHealScript(Spell subject) : base(subject)
        {
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

        protected GroupComponent GroupComponent{ get; }
        protected GroupComponent.GroupComponentOptions GroupComponentOptions { get; }

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

            var targets = GroupComponent.Activate<Aisling>(context, GroupComponentOptions);
            GroupComponent.ApplyHealing(context, targets.targetEntities, GroupComponentOptions);
        }
        
        #region ScriptVars
        protected int? ManaSpent { get; init; }
        protected int? BaseHealing { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        
        #endregion
    }
}
