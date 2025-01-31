using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.TrialOfSacrifice.sacrificemob;

public class SacrificeMobAggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private readonly IIntervalTimer LastHitTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public SacrificeMobAggroTargetingScript(Monster subject)
        : base(subject)
    {
        TargetUpdateTimer =
            new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(250, Subject.Template.SkillIntervalMs)));
        
        LastHitTimer =
            new IntervalTimer(TimeSpan.FromSeconds(5));
    }


    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        var zoeaggro = 100000000; // Set a fixed aggro value for the Totem
        var helplesszoe = Map.GetEntitiesWithinRange<Monster>(Subject, 20)
                       .ThatAreVisibleTo(Subject)
                       .Where(obj => obj.Template.Name == "Helpless Zoe")
                       .ClosestOrDefault(Subject);
        
        base.Update(delta);

        TargetUpdateTimer.Update(delta);
        LastHitTimer.Update(delta);
        
        if (LastHitTimer.IntervalElapsed)
        {
            AggroList.Clear();
            LastHitTimer.Reset();
            if (helplesszoe != null)
            {
                AggroList.AddOrUpdate(helplesszoe.Id, _ => zoeaggro, (_, currentAggro) => currentAggro + zoeaggro);
            }
        }

        if ((Target != null) && (!Target.IsAlive || !Target.OnSameMapAs(Subject)))
        {
            AggroList.Remove(Target.Id, out _);
            Target = null;
        }

        if (!TargetUpdateTimer.IntervalElapsed)
            return;

        Target = null;

        if (!Map.GetEntities<Aisling>().Any())
            return;

        var isBlind = Subject.IsBlind;

        //first try to get target via aggro list
        //if something is already aggro, ignore aggro range
        foreach (var kvp in AggroList.OrderByDescending(kvp => kvp.Value))
        {
            if (!Map.TryGetEntity<Creature>(kvp.Key, out var possibleTarget))
                continue;

            if (!possibleTarget.IsAlive || !Subject.CanSee(possibleTarget) || !possibleTarget.WithinRange(Subject))
                continue;

            //if we're blind, we can only target things within 1 tile
            if (isBlind && !possibleTarget.WithinRange(Subject, 1))
                continue;

            Target = possibleTarget;

            break;
        }

        if (Target != null)
            return;

        //if blind, we can only target things within 1 space
        var range = isBlind ? 1 : AggroRange;

        //if we failed to get a target via aggroList, grab the closest aisling within aggro range
        Target ??= Map.GetEntitiesWithinRange<Aisling>(Subject, range)
                      .ThatAreVisibleTo(Subject)
                      .Where(
                          obj => !obj.Equals(Subject)
                                 && obj.IsAlive
                                 && Subject.ApproachTime.TryGetValue(obj, out var time)
                                 && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
                      .ClosestOrDefault(Subject);

        //since we grabbed a new target, give them some initial aggro so we stick to them
        if (Target != null)
            AggroList[Target.Id] = InitialAggro++;
    }
}