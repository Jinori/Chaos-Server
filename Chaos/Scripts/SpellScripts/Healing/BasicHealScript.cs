using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts.Healing
{
    internal class BasicHealScript : BasicSpellScriptBase
    {
        protected int? BaseHealing { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        protected int? manaSpent { get; init; }


        public BasicHealScript(Spell subject) : base(subject)
        {
        }

        protected virtual void ApplyHealing(SpellContext context, IEnumerable<Creature> targetEntities)
        {
            foreach (var target in targetEntities)
            {
                var heals = CalculateHealing(context, target);
                target.ApplyHealing(target, heals);
            }
        }

        protected virtual int CalculateHealing(SpellContext context, Creature target)
        {
            var heals = BaseHealing ?? 0;

            if (HealStat.HasValue)
            {
                var multiplier = HealStatMultiplier ?? 1;

                heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
            }

            return heals;
        }

        /// <inheritdoc />
        protected override IEnumerable<T> GetAffectedEntities<T>(SpellContext context, IEnumerable<IPoint> affectedPoints)
        {
            var entities = base.GetAffectedEntities<T>(context, affectedPoints);

            return entities;
        }

        /// <inheritdoc />
        public override void OnUse(SpellContext context)
        {
            if (manaSpent.HasValue)
            {
                //Require mana
                if (context.Source.StatSheet.CurrentMp < manaSpent.Value)
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                    return;
                }
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(manaSpent.Value);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }

            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);
            ApplyHealing(context, affectedEntities);
        }
    }
}
