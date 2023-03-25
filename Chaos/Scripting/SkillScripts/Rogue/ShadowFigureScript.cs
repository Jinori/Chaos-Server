using Chaos.Data;
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
        if (targetCreature == null || context.Map.IsWall(targetCreature) || context.Map.IsBlockingReactor(targetCreature))
            return;
        // Get the direction behind the target creature
        var behindTargetDirection = targetCreature.DirectionalRelationTo(context.SourcePoint);
        // Check if the destination point is valid
        var destinationPoint = targetCreature.DirectionalOffset(behindTargetDirection);
        if (context.Map.IsWalkable(destinationPoint, context.Source.Type))
        {
            context.Source.WarpTo(destinationPoint);
            var newDirection = targetCreature.DirectionalRelationTo(context.Source);
            context.Source.Turn(newDirection);
        }
        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
    }
}