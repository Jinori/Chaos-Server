using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct AnimationAbilityComponent2 : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IAnimationComponentOptions>();
        var points = vars.GetPoints();
        var targets = vars.GetTargets<MapEntity>();

        if (options.Animation2 == null)
            return;

        if (options.AnimatePoints2)
            foreach (var point in points)
                context.TargetMap.ShowAnimation(options.Animation2.GetPointAnimation(point, context.Source.Id));
        else
            foreach (var target in targets)
                target.Animate(options.Animation2, context.Source.Id);
    }

    public interface IAnimationComponentOptions
    {
        bool AnimatePoints2 { get; init; }
        Animation? Animation2 { get; init; }
    }
}