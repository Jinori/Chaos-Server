using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.ServantBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class ServantDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public ServantDeathScript(Monster subject)
        : base(subject)
    {
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
            rewardTargets = (rewardTarget.Group ?? (IEnumerable<Aisling>)new[] { rewardTarget })
                .ThatAreWithinRange(rewardTarget)
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

            var aislings = Subject.MapInstance.GetEntities<Aisling>().Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor) || x.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3)).ToList();

            foreach (var aisling in aislings)
            {
                if (aisling.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor))
                {
                    aisling.Trackers.Enums.Set(MainStoryEnums.DefeatedServant);
                    aisling.Trackers.Flags.AddFlag(MainstoryFlags.CompletedFloor3);
                    aisling.Inventory.RemoveQuantityByTemplateKey("trueelementalartifact", 1);
                    aisling.SendOrangeBarMessage("The servant collapses. Return to Goddess Miraelis");
                }
                else if (aisling.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3))
                {
                    aisling.SendOrangeBarMessage("Thank you for helping others.");
                    aisling.TryGiveGamePoints(25);
                    aisling.TryGiveGold(100000);
                    ExperienceDistributionScript.GiveExp(aisling, 350000);
                }
            }
        }
    }
}