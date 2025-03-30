using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class CasterMovementScript : MonsterScriptBase
{
    /// <inheritdoc />
    public CasterMovementScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())

            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        if (distance <= 5)
        {
            var pathtopoint = Subject.SpiralSearch(3)
                                     .OrderByDescending(point => point.ManhattanDistanceFrom(Target))
                                     .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, collisionType: Subject.Type));

            Subject.Pathfind(pathtopoint);
        } else
            Subject.Pathfind(Target);

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}