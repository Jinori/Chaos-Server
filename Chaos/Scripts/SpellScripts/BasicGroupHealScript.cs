using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts
{
    internal class BasicGroupHealScript : BasicSpellScriptBase
    {
        protected int? BaseHealing { get; init; }
        protected Stat? HealStat { get; init; }
        protected decimal? HealStatMultiplier { get; init; }
        protected int? manaSpent { get; init; }


        public BasicGroupHealScript(Spell subject) : base(subject)
        {
        }

        protected virtual int CalculateHealing(SpellContext context, Creature target)
        {
            var heals = BaseHealing ?? 0;

            if (HealStat.HasValue)
            {
                if (context.Source == target)
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
            //Status Related
            if (context.Source.Status.HasFlag(Status.Suain))
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your hands are frozen.");
                return;
            }

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

            var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));

            if (group is null && BaseHealing.HasValue)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are not in a group.");
                context.SourceAisling?.ApplyHealing(context.Source, CalculateHealing(context, context.Source));
                ShowBodyAnimation(context);
                PlaySound(context, context.SourcePoint);
                return;
            }
            if (group is not null && BaseHealing.HasValue)
            {
                group.ToList().ForEach(n => n.ApplyHealing(n, CalculateHealing(context, n)));
                PlaySound(context, context.TargetPoint);
                ShowAnimation(context, group);
                ShowBodyAnimation(context);
            }
        }
    }
}
