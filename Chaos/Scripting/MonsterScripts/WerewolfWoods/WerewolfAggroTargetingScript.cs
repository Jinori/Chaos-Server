using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.WerewolfWoods;

// ReSharper disable once ClassCanBeSealed.Global
public class WerewolfAggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public WerewolfAggroTargetingScript(Monster subject)
        : base(subject)
        => TargetUpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(250, Subject.Template.SkillIntervalMs)));

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        if (source.Equals(Subject))
            return;

        var aggro = aggroOverride ?? damage;

        if (aggro == 0)
            return;
        
        source.Trackers.Enums.Set(AttackedWerewolf.Yes);

        // Always add to aggro list when attacked
        AggroList.AddOrUpdate(source.Id, _ => aggro, (_, currentAggro) => currentAggro + aggro);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        TargetUpdateTimer.Update(delta);

        if ((Target != null) && (!Target.IsAlive || !Target.OnSameMapAs(Subject) || Target.MapInstance.IsWalkable(Target, CreatureType.Normal)))
        {
            Target.Trackers.Enums.Set(AttackedWerewolf.No);
            AggroList.Remove(Target.Id, out _);
            Target = null;
        }

        if (!TargetUpdateTimer.IntervalElapsed)
            return;

        Target = null;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var isBlind = Subject.IsBlind;

        //first try to get target via aggro list
        //if something is already aggro, ignore aggro range
        foreach (var kvp in AggroList.OrderByDescending(kvp => kvp.Value))
        {
            if (!Map.TryGetEntity<Creature>(kvp.Key, out var possibleTarget))
                continue;

            if (!possibleTarget.IsAlive || !Subject.CanSee(possibleTarget) || !possibleTarget.WithinRange(Subject) || Subject.MapInstance.IsWalkable(possibleTarget, Subject.Type))
                continue;

            //if we're blind, we can only target things within 1 tile
            if (isBlind && !possibleTarget.WithinRange(Subject, 1))
                continue;

            // Check if the target has the werewolf effect
            if (possibleTarget is Aisling aisling && aisling.Effects.Contains("Werewolf") && !possibleTarget.Trackers.Enums.HasValue(AttackedWerewolf.Yes))
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
                                 && !obj.Effects.Contains("werewolf")
                                 || obj.Trackers.Enums.HasValue(AttackedWerewolf.Yes)// Exclude Aislings with the werewolf effect
                                 && Subject.ApproachTime.TryGetValue(obj, out var time)
                                 && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
                      .ClosestOrDefault(Subject);

        //since we grabbed a new target, give them some initial aggro so we stick to them
        if (Target != null)
            AggroList[Target.Id] = InitialAggro++;
    }
}
