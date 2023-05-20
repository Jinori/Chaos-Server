using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;


namespace Chaos.Scripting.SkillScripts.Monk;



public class AmbushScript : SkillScriptBase
{
    /// <inheritdoc />
    public AmbushScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        //get the 3 points in front of the source
        var endPoint = context.Source.DirectionalOffset(context.Source.Direction, 3);

        var points = context.Source.GetDirectPath(endPoint)
                            .Skip(1);

        foreach (var point in points)
        {
            if (context.TargetMap.IsWall(point) || context.TargetMap.IsBlockingReactor(point))
                return;

            var entity = context.TargetMap.GetEntitiesAtPoint<Creature>(point)
                                .TopOrDefault();


        }
    }
}