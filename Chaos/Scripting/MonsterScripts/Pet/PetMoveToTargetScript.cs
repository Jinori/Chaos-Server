using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

// ReSharper disable once ClassCanBeSealed.Global
public class PetMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PetMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Subject.PetOwner?.ManhattanDistanceFrom(Subject) >= 8)
        {
            Target = null;

            return;
        }

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Monster>().Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        if (distance != 1)
            Subject.Pathfind(Target);
        else
        {
            var direction = Target.DirectionalRelationTo(Subject);
            Subject.Turn(direction);
        }

        Subject.SkillTimer.Reset();
    }
}