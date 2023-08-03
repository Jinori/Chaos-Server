using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Utilities;
using IComponent = Chaos.Scripting.Components.Abstractions.IComponent;

namespace Chaos.Scripting.Components;

public class ThrowComponent : IComponent
{
    private Animation ThrowAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 123
    };
    
    public interface IThrowComponentOptions
    {
        int DistanceToThrow { get; set; }
    }

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targetPoint = context.Source.DirectionalOffset(context.TargetDirection);
    
        var entity = context.TargetMap.GetEntitiesAtPoint<Creature>(targetPoint)
                            .TopOrDefault();

        if (entity != null)
        {
            var throwDirection = context.TargetDirection;
            var throwPoint = entity.DirectionalOffset(throwDirection);
        
            if (!entity.MapInstance.IsWall(throwPoint) && !entity.MapInstance.IsBlockingReactor(throwPoint))
            {
                entity.WarpTo(throwPoint);
            }
            entity.MapInstance.ShowAnimation(ThrowAnimation.GetPointAnimation(entity));
        }
    }
}