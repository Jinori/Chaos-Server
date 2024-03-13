using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.DragonScaleBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class dragonScaleDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public dragonScaleDeathScript(Monster subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
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

            var nearbyAislingsWithDragon = Map
                .GetEntitiesWithinRange<Aisling>(Subject, 13)
                .FirstOrDefault(x => x.Trackers.Enums.TryGetValue(out DragonScale stage) && stage == DragonScale.SpawnedDragon);
            
            if (nearbyAislingsWithDragon != null)
            {
                var dragonscale = ItemFactory.Create("dragonscale");
                
                foreach (var target in rewardTargets)
                {
                    target.Trackers.Enums.Set(DragonScale.KilledDragon);
                    target.SendOrangeBarMessage("The dragon collapses, you scour the body for a scale.");
                    target.TryGiveItem(ref dragonscale);
                }
            }
        }
    }
}