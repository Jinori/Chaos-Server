using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents
{
    public struct ThrowCreatureComponent : IComponent
    {
        /// <inheritdoc />
        public void Execute(ActivationContext context, ComponentVars vars)
        {
            var options = vars.GetOptions<IDamageComponentOptions>();
            var targets = vars.GetTargets<Creature>();

            var throwDirection = context.Source.Direction;
            var throwRange = options.ThrowRange ?? 1; // Default to 1 if not specified

            // Calculate the target point based on the throw direction and range
            var thrownPoint = context.Source.DirectionalOffset(throwDirection, throwRange);
            var targetPoint = thrownPoint.DirectionalOffset(throwDirection);

            // Potential points are the throw point, and the 3 points around it
            var potentialTargetPoints = targetPoint.GenerateCardinalPoints()
                .WithConsistentDirectionBias(throwDirection)
                .SkipLast(1)
                .Prepend(targetPoint)
                .ToList();

            foreach (var target in targets)
            {
                foreach (var point in potentialTargetPoints)
                {
                    if (context.SourceMap.IsWalkable(point, CreatureType.Aisling, false))
                    {
                        if (!target.IsHostileTo(context.Source))
                            continue;

                        var aislingPoint = Point.From(target);

                        if (context.SourceMap.IsWall(targetPoint))
                            continue;

                        target.WarpTo(point);
                        break;
                    }
                }
            }
        }

        public interface IDamageComponentOptions
        {
            int? ThrowRange { get; init; }
        }
    }
}
