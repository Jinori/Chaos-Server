using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossMoveToTargetScript(Monster subject, IEffectFactory effectFactory) : MonsterScriptBase(subject)
{
    /// <inheritdoc />
    ///
    ///

    private readonly IEffectFactory EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>().Any())
            return;

        if ((Target.Trackers.LastWalk != null) && (Target.Trackers.LastWalk.Value < DateTime.UtcNow.AddSeconds(-5)))
        {
            var point = Subject.GenerateCardinalPoints().FirstOrDefault();
            Target.TraverseMap(Subject.MapInstance, point);
            Target.Trackers.LastWalk = DateTime.UtcNow;
            var effect = EffectFactory.Create("earthpunch");
            Target.Effects.Apply(Subject, effect);
        }
        
        var distance = Subject.DistanceFrom(Target);

        if ((distance != 1) && (distance <= 3))
        {
            Subject.Pathfind(Target);
        }
        
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
                                         && Subject.ApproachTime.TryGetValue(obj.Id, out var time)
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