using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

// ReSharper disable once ClassCanBeSealed.Global
public class TeammateMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TeammateMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Subject.PetOwner?.DistanceFrom(Subject) >= 8)
        {
            Target = null;

            return;
        }

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Monster>().Any())
            return;

        var distance = Subject.DistanceFrom(Target);

        if (Subject.Template.TemplateKey.Contains("wizard"))
        {
            if (distance <= 2)
            {
                var pathtopoint = Subject.SpiralSearch(3).OrderByDescending(point => point.DistanceFrom(Target))
                    .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, Subject.Type));
            
                Subject.Pathfind(pathtopoint);
            }
        }
        else
        {
            if (distance != 1)
                Subject.Pathfind(Target);
            else
            {
                var direction = Target.DirectionalRelationTo(Subject);
                Subject.Turn(direction);
            } 
        }

        Subject.SkillTimer.Reset();
    }
}