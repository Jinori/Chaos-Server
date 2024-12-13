using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events.BabyErbie;

public class BabyErbieDefenseMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public BabyErbieDefenseMoveToTargetScript(Monster subject)
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

        if (distance <= 9)
        {
            var pathtopoint = Subject.SpiralSearch(3)
                                     .OrderByDescending(point => point.ManhattanDistanceFrom(Target))
                                     .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, Subject.Type));

            Subject.Pathfind(pathtopoint);
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}