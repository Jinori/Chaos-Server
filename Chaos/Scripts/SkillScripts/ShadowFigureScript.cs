﻿using Chaos.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Objects;
using Chaos.Scripts.SkillScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Common.Definitions;

namespace Chaos.Scripts.SkillScripts
{
    public class ShadowFigureScript : BasicSkillScriptBase
    {
        protected int? BaseDamage { get; init; }
        protected Stat? DamageStat { get; init; }
        protected decimal? DamageStatMultiplier { get; init; }

        /// <inheritdoc />
        public ShadowFigureScript(Skill subject)
            : base(subject) { }

        /// <inheritdoc />
        public override void OnUse(SkillContext context)
        {
            //get the 3 points in front of the source
            var points = AoeShape.Front.ResolvePoints(
                context.SourcePoint,
                1,
                context.Source.Direction,
                context.Map.Template.Bounds);

            //get the first creature along those points
            var targetCreature = context.Map.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                                        .FirstOrDefault();

            //if there is no creature, return
            if (targetCreature == null)
                return;

            //if the target is standing on a wall, return
            if (context.Map.IsWall(targetCreature))
                return;

            //if any point on the path to the creature is a wall, return
            if (context.Source.GetDirectPath(targetCreature).Any(pt => context.Map.IsWall(pt)))
                return;

            //get the direction that vectors behind the target relative to the source
            var behindTargetDirection = targetCreature.DirectionalRelationTo(context.SourcePoint);

            //for each direction around the target, starting with the direction behind the target
            foreach (var direction in behindTargetDirection.AsEnumerable())
            {
                //get the point in that direction
                var destinationPoint = targetCreature.DirectionalOffset(direction);
                //if that point is not talkable, continue
                if (!context.Map.IsWalkable(destinationPoint, context.Source.Type))
                    continue;

                //if it is walkable, warp to that point and turn to face the target
                context.Source.WarpTo(destinationPoint);
                var newDirection = targetCreature.DirectionalRelationTo(context.Source);
                context.Source.Turn(newDirection);
                ApplyDamage(context, targetCreature);
                ShowBodyAnimation(context);
                return;
            }
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
        protected virtual void ApplyDamage(SkillContext context, Creature targetCreature)
        {
            var damage = CalculateDamage(context, targetCreature);
            targetCreature.ApplyDamage(context.Source, damage);
        }
    }
}