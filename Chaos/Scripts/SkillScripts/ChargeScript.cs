using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SkillScripts
{
    public class ChargeScript : BasicSkillScriptBase
    {
        protected int? BaseDamage { get; init; }
        protected Stat? DamageStat { get; init; }
        protected decimal? DamageStatMultiplier { get; init; }


        public ChargeScript(Skill subject) : base(subject)
        {
        }

        public override void OnUse(SkillContext context)
        {
            //get the 3 points in front of the source
            var points = AoeShape.Front.ResolvePoints(
                context.SourcePoint,
                5,
                context.Source.Direction,
                context.Map.Template.Bounds);


            var targetCreature = context.Map.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                            .FirstOrDefault();

            if (targetCreature is null)
            {
                //get the point in that direction
                var destinationPoint = context.SourcePoint.GetDirectPath(points.Last());

                //if that point is not walkable, continue
                if (!context.Map.IsWalkable(points.Last(), context.Source.Type))
                    return;

                //if it is walkable, warp to that point
                context.Source.WarpTo(points.Last());
            }

            if (targetCreature is not null)
            {

            }

            ApplyDamage(context, targetCreature);
            ShowBodyAnimation(context);

            return;
        }

        private void ApplyDamage(SkillContext context, Creature targetCreature)
        {
            var damage = CalculateDamage(context, targetCreature);
            targetCreature.ApplyDamage(context.Source, damage);
        }

        protected virtual int CalculateDamage(SkillContext context, Creature targetCreature)
        {
            var damage = BaseDamage ?? 0;

            if (DamageStat.HasValue)
            {
                var multiplier = DamageStatMultiplier ?? 1;

                damage += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(DamageStat.Value) * multiplier);
            }

            return DamageFormulae.Default.Calculate(context.Source, targetCreature, damage);
        }
    }
}
