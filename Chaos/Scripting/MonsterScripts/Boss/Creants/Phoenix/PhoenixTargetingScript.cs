using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

// phoenix ignores all aggro
// she will randomly target things for 5s at a time
// she will prefer to attack people standing in a cluster

public class PhoenixTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(250));
    private readonly IIntervalTimer TargetChangeTimer = new IntervalTimer(TimeSpan.FromSeconds(5));

    /// <inheritdoc />
    public PhoenixTargetingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (Map.LoadedFromInstanceId.ContainsI("sky"))
            return;

        TargetUpdateTimer.Update(delta);
        TargetChangeTimer.Update(delta);

        if ((Target != null)
            && (!Target.IsAlive
                || !Target.OnSameMapAs(Subject)
                || Target.MapInstance.IsWalkable(Target, CreatureType.Normal)
                || !Subject.CanObserve(Target)
                || !Subject.CanSee(Target)
                || Subject.IsGodModeEnabled()
                || !Subject.IsAlive))
            Target = null;

        if (TargetUpdateTimer.IntervalElapsed)
            if (Target is null || Target.IsDead)
                Target = Map.GetEntitiesWithinRange<Aisling>(Subject)
                            .ThatAreObservedBy(Subject)
                            .ThatAreVisibleTo(Subject)
                            .ThatAreNotInGodMode()
                            .ThatAreAlive()
                            .Shuffle()
                            .FirstOrDefault();

        if (TargetChangeTimer.IntervalElapsed)
        {
            //get nearby aislings
            var nearbyAislings = Map.GetEntitiesWithinRange<Aisling>(Subject)
                                    .ThatAreObservedBy(Subject)
                                    .ThatAreVisibleTo(Subject)
                                    .ThatAreNotInGodMode()
                                    .ThatAreAlive()
                                    .ToList();

            //group by cluster size
            var clusters = nearbyAislings.GroupBy(aisling => nearbyAislings.Count(other => aisling.WithinRange(other, 3)));

            //order by biggest to smallest cluster
            //then randomly select one of the ones with the biggest cluster size
            Target = clusters.OrderByDescending(group => group.Key)
                             .ThenBy(_ => Random.Shared.Next())
                             .FirstOrDefault()
                             ?.FirstOrDefault();
        }
    }
}