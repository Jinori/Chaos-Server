using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.KarloposBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class QueenOctopusDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public QueenOctopusDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        //this code will set the reward target to the person at the top of the aggro list
        //var rewardTarget = Subject.AggroList
        //                          .OrderByDescending(kvp => kvp.Value)
        //                          .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
        //                          .FirstOrDefault(a => a is not null);

        //get the highest contributor
        //if there are no contributor, try getting the highest aggro
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)
            rewardTargets = (rewardTarget.Group
                             ?? (IEnumerable<Aisling>)new[]
                             {
                                 rewardTarget
                             }).ThatAreWithinRange(rewardTarget)
                               .ToArray();

        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
        var droppedITems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        if (rewardTargets is not null)
        {
            if (WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
            {
                var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

                if (droppedGold)
                    money!.LockToAislings(lockSecs, rewardTargets);

                if (droppedITems)
                    foreach (var groundItem in groundItems!)
                        groundItem.LockToAislings(lockSecs, rewardTargets);
            }

            ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
            
            var rectangle = new Rectangle(
                21,
                5,
                2,
                2);

            foreach (var member in Subject.MapInstance.GetEntities<Aisling>().ToList())
            {
                var mapInstance = SimpleCache.Get<MapInstance>("karloposn");
                Point point;

                do
                    point = rectangle.GetRandomPoint();
                while (!mapInstance.IsWalkable(point, member.Type));

                var hasStage = member.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);

                if (stage is QueenOctopusQuest.SpokeToMaria or QueenOctopusQuest.QueenSpawning or QueenOctopusQuest.QueenSpawned)
                    member.Trackers.Enums.Set(QueenOctopusQuest.QueenKilled);

                member.TraverseMap(mapInstance, point);
            }
        }
    }
}