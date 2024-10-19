using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Priest;

public class LimboPriestDefenseMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public LimboPriestDefenseMoveToTargetScript(Monster subject)
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
                                     .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, Subject.Type));

            Subject.Pathfind(pathtopoint);
        } else
            Subject.Pathfind(Target);

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}