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

namespace Chaos.Scripts.SpellScripts
{
    internal class SalvationScript : BasicSpellScriptBase
    {

        public SalvationScript(Spell subject) : base(subject)
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
            var heals = target.StatSheet.MaximumHp;

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
            //Require mana
            if (context.Source.StatSheet.CurrentMp < 1)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                return;
            }
            if (context.SourceAisling?.StatSheet.CurrentMp >= 1)
            {
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(context.Source.StatSheet.MaximumMp);
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