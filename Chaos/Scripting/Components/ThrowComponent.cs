using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ThrowComponent : IComponent
{
    private Animation ThrowAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 123
    };

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targetPoint = context.Source.DirectionalOffset(context.TargetDirection);

        var entity = context.TargetMap.GetEntitiesAtPoint<Creature>(targetPoint)
                            .TopOrDefault();

        if (entity != null)
        {
            if (entity.Trackers.Enums.TryGetValue(out GodMode god) && (god == GodMode.Yes))
                return;
            
            var throwDirection = context.TargetDirection;
            var throwPoint = entity.DirectionalOffset(throwDirection);

            if (!entity.MapInstance.IsWall(throwPoint) && !entity.MapInstance.IsBlockingReactor(throwPoint))
                entity.WarpTo(throwPoint);

            entity.MapInstance.ShowAnimation(ThrowAnimation.GetPointAnimation(entity));
        }
    }

    public interface IThrowComponentOptions
    {
        int DistanceToThrow { get; set; }
    }
}