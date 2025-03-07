#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SkillScripts.Abstractions;
#endregion

namespace Chaos.Scripting.SkillScripts.Monk;

public class ThrowScript : SkillScriptBase
{
    private readonly Animation ThrowAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 123
    };

    /// <inheritdoc />
    public ThrowScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var throwDirection = context.Source.Direction;
        var thrownPoint = context.Source.DirectionalOffset(throwDirection);
        var thrownAislings = context.TargetMap.GetEntitiesAtPoints<Aisling>(thrownPoint);
        var targetPoint = thrownPoint.DirectionalOffset(throwDirection);

        //potential points are the throw point, and the 3 points around it
        var potentialTargetPoints = targetPoint.GenerateCardinalPoints()
                                               .WithConsistentDirectionBias(throwDirection)
                                               .SkipLast(1)
                                               .Prepend(targetPoint)
                                               .ToList();

        foreach (var aisling in thrownAislings)
        {
            if (aisling.IsGodModeEnabled())
                continue;

            if (!aisling.IsAlive)
                continue;

            if ((context.SourceAisling != null)
                && (aisling.Effects.Contains("Fishing")
                    || (aisling.Effects.Contains("Foraging") && !context.SourceAisling.IsGodModeEnabled())))
            {
                context.SourceAisling.SendOrangeBarMessage("That player is currently busy and cannot be thrown.");

                continue;
            }

            foreach (var point in potentialTargetPoints)
                if (context.SourceMap.IsWalkable(point, CreatureType.Aisling, false))
                {
                    var aislingPoint = Point.From(aisling);

                    context.SourceMap.ShowAnimation(ThrowAnimation.GetPointEffectAnimation(aislingPoint));

                    if (context.SourceMap.IsWall(targetPoint))
                        continue;

                    context.SourceMap.ShowAnimation(ThrowAnimation.GetPointEffectAnimation(point));
                    aisling.WarpTo(point);

                    break;
                }
        }
    }
}