using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class ChargeScript : DamageScript
{
    private new IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public ChargeScript(Skill subject)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        // Get the endpoint 4 tiles away from the source in the same direction
        var endPoint = context.Source.DirectionalOffset(context.Source.Direction, 4);
        // Get all points between the source and the endpoint, skipping the first one
        var points = context.Source.GetDirectPath(endPoint).Skip(1);
        // Iterate through each point
        foreach (var point in points)
        {
            // If there is a wall at this point, return
            if (context.SourceMap.IsWall(point) || context.SourceMap.IsBlockingReactor(point))
            {
                var distance = point.DistanceFrom(context.Source);
                if (distance == 1)
                    return;
                var newPoint = point.OffsetTowards(context.Source);
                context.Source.WarpTo(newPoint);
                return;
            }
            // Get the closest creature to the source at this point
            var entity = context.SourceMap.GetEntitiesAtPoint<Creature>(point).OrderBy(creature => creature.DistanceFrom(context.Source))
                .FirstOrDefault(creature => context.Source.WillCollideWith(creature));
            // If there is a creature at this point
            if (entity != null)
            {
                // Get the point towards the source from the creature
                var newPoint = entity.OffsetTowards(context.Source);
                // Warp the source to the new point
                context.Source.WarpTo(newPoint);
                
                // Needs Damage & Ability Component or a new Movement Component
                return;
            }
        }

        // If no creature was found, warp the source to the endpoint
        context.Source.WarpTo(endPoint);
    }
}