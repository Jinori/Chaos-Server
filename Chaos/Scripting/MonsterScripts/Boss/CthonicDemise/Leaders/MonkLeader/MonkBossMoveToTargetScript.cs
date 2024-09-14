using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CthonicDemise.Leaders.MonkLeader;

public sealed class MonkBossMoveToTargetScript(Monster subject) : MonsterScriptBase(subject)
{
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>().Any())
            return;
        
        var distance = Subject.ManhattanDistanceFrom(Target);


        var point = Target.GenerateCardinalPoints().OfType<IPoint>().FirstOrDefault(x => Subject.MapInstance.IsWalkable(x, Subject.Type));
        //Remove aggro if we can't get to the target so we have a chance to attack someone else
        if (point is null && (Map.GetEntities<Aisling>().Count() > 1))
        {
            AggroList.Remove(Target.Id, out _);
            Target = null;
            return;
        }

        if ((distance != 1) && (distance <= 4))
            Subject.Pathfind(Target);

        else if (distance >= 4)
        {
            var safeSpot = Target.GenerateCardinalPoints().OfType<IPoint>().FirstOrDefault(x => Subject.MapInstance.IsWalkable(x, Subject.Type));

            if (safeSpot is null)
            {
                AggroList.Remove(Target.Id, out _);
                Target ??= Map.GetEntitiesWithinRange<Aisling>(Subject, 12)
                              .ThatAreVisibleTo(Subject)
                              .Where(
                                  obj => !obj.Equals(Subject)
                                         && obj.IsAlive
                                         && Subject.ApproachTime.TryGetValue(obj, out var time)
                                         && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
                              .ClosestOrDefault(Subject);
                return;
            }
            
            Subject.TraverseMap(Target.MapInstance, safeSpot);
        }
        else
        {
            var direction = Target.DirectionalRelationTo(Subject);
            Subject.Turn(direction);
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}