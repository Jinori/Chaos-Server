using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using System.Collections.Generic;
using Chaos.Extensions;

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

            foreach (var target in targets)
            {
                // Calculate the target point based on the throw direction and range relative to the target's position
                var targetCurrentPoint = Point.From(target);
                var targetThrowPoint = targetCurrentPoint.DirectionalOffset(throwDirection, throwRange);
                var potentialTargetPoints = targetThrowPoint.GenerateCardinalPoints()
                    .WithConsistentDirectionBias(throwDirection)
                    .SkipLast(1)
                    .Prepend(targetThrowPoint)
                    .ToList();

                if (target.StatSheet.CurrentHp < 1)
                    return;

                if (target.IsGodModeEnabled())
                    return;

                foreach (var point in potentialTargetPoints)
                {
                    if (!target.IsHostileTo(context.Source))
                        continue;

                    if (context.SourceMap.IsWall(point))
                        break;

                    
                    if (context.SourceMap.IsWalkable(point, CreatureType.Aisling, false))
                    {
                        // Warp the target to the point and add it to the set of thrown targets
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
