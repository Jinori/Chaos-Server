using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public BossMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>().Any())
            return;

        var distance = Subject.DistanceFrom(Target);

        if ((distance != 1) && (distance <= 3))
            Subject.Pathfind(Target);
        
        else if (distance >= 4)
        {
            var safeSpot = Target.GenerateCardinalPoints().OfType<IPoint>().FirstOrDefault(x => Subject.MapInstance.IsWalkable(x, Subject.Type));

            if (safeSpot is null)
                return;
            
            Subject.TraverseMap(Target.MapInstance, safeSpot);
            
            var newDirection = Target.DirectionalRelationTo(Subject);
            Subject.Turn(newDirection);
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}