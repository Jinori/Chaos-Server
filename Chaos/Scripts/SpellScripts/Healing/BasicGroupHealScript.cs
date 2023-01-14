using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Data;

namespace Chaos.Scripts.SpellScripts.Healing
{
    internal class BasicGroupHealScript : BasicSpellScriptBase
    {
        public BasicGroupHealScript(Spell subject) : base(subject)
        {
        }

        protected virtual int CalculateHealing(SpellContext context, Creature target)
        {
            var heals = BaseHealing ?? 0;

            if (HealStat.HasValue)
            {
                if (context.Source.Equals(target))
                {
                    var multiplier = HealStatMultiplier ?? 1;

                    heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
                    //Divide by two for healing yourself
                    heals = Convert.ToInt32(heals / 2);
                }
                else
                {
                    var multiplier = HealStatMultiplier ?? 1;

                    heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
                }
            }

            return heals;
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
            
            var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));

            if (group is null && BaseHealing.HasValue)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are not in a group.");
                context.SourceAisling?.ApplyHealing(context.Source, CalculateHealing(context, context.Source));
                return;
            }
            if (group is not null && BaseHealing.HasValue)
            {
                group.ToList().ForEach(n => n.ApplyHealing(n, CalculateHealing(context, n)));
            }
        }
        
        #region ScriptVars
        protected int? BaseHealing { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        protected int? ManaSpent { get; init; }
        #endregion
    }
}
