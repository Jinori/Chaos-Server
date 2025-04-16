using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.MonsterScripts.Boss;

namespace Chaos.Scripting.Components.AbilityComponents;

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
            if (options.AllAround)
                throwDirection = target.DirectionalRelationTo(context.Source);

            // Calculate the target point based on the throw direction and range relative to the target's position
            var targetCurrentPoint = Point.From(target);
            var targetThrowPoint = targetCurrentPoint.DirectionalOffset(throwDirection, throwRange);

            var potentialTargetPoints = targetThrowPoint.GenerateCardinalPoints()
                                                        .WithConsistentDirectionBias(throwDirection)
                                                        .SkipLast(1)
                                                        .Prepend(targetThrowPoint)
                                                        .ToList();

            if (target.Script.Is<ThisIsABossScript>())
                continue;

            if (target.StatSheet.CurrentHp < 1)
                continue;

            if (target.IsGodModeEnabled())
                continue;

            foreach (var point in potentialTargetPoints)
            {
                if (!target.IsHostileTo(context.Source))
                    continue;

                if (context.SourceMap.IsWall(point))
                    break;

                if (context.SourceMap.IsWalkable(point, collisionType: CreatureType.Aisling))
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
        public bool AllAround { get; init; }
        int? ThrowRange { get; init; }
    }
}