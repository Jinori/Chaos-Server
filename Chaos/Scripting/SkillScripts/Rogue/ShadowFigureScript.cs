﻿using Chaos.Data;
using Chaos.Extensions.Geometry;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class ShadowFigureScript : DamageScript
{
    /// <inheritdoc />
    public ShadowFigureScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        var targetCreature = targets.TargetEntities.FirstOrDefault();

        //if there is no creature, return
        if (targetCreature == null)
            return;

        //if the target is standing on a wall, return
        if (context.Map.IsWall(targetCreature))
            return;

        //get the direction that vectors behind the target relative to the source
        var behindTargetDirection = targetCreature.DirectionalRelationTo(context.SourcePoint);

        //for each direction around the target, starting with the direction behind the target
        foreach (var direction in behindTargetDirection.AsEnumerable())
        {
            //get the point in that direction
            var destinationPoint = targetCreature.DirectionalOffset(direction);

            //if that point is not walkable, continue
            if (!context.Map.IsWalkable(destinationPoint, context.Source.Type))
                continue;

            //if it is walkable, warp to that point and turn to face the target
            context.Source.WarpTo(destinationPoint);
            var newDirection = targetCreature.DirectionalRelationTo(context.Source);
            context.Source.Turn(newDirection);

            break;
        }

        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
    }
}