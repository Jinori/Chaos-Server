using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.WestWoodlands;

// ReSharper disable once ClassCanBeSealed.Global
public class WWBossDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly IReactorTileFactory ReactorTileFactory;

    /// <inheritdoc />
    public WWBossDeathScript(Monster subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
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

        if (rewardTarget?.Group != null)
        {
            switch (rewardTarget.Group.LootOption)
            {
                case Group.GroupLootOption.Default:
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
                    }
                    break;
                case Group.GroupLootOption.Random:
                    if (rewardTargets is not null)
                    {
                        if (Subject.Items.Count >= 1) 
                            rewardTarget.Group.DistributeRandomized(Subject.Items);
                    
                        if (Subject.Gold >= 1) 
                            rewardTarget.Group.DistributeEvenGold(Subject.Gold);
                    
                        ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);   
                    }
                    break;
                case Group.GroupLootOption.MasterLooter:
                    var droppedGold1 = Subject.TryDropGold(Subject, Subject.Gold, out var money1);
                    var droppedITems1 = Subject.TryDrop(Subject, Subject.Items, out var groundItems1);
                    if (rewardTargets is not null)
                    {
                        if (WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
                        {
                            var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

                            if (droppedGold1)
                                money1!.LockToAislings(lockSecs, rewardTarget.Group.Leader);

                            if (droppedITems1)
                                foreach (var groundItem in groundItems1!)
                                    groundItem.LockToAislings(lockSecs, rewardTarget.Group.Leader);
                        }

                        ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
                    }
                    break;
            }
        }
        else
        {
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
            }
        }

        if (Subject.MapInstance.GetEntities<Monster>()
            .All(x => x.Name != "Elder Mage" && x.Name != "Fire Draco" && x.Name != "Twink" && x.Name != "Grunk"))
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(WestWoodlandsDungeonQuestStage.Started)).ToList();

            foreach (var aisling in aislings)
            {
                aisling.Trackers.Enums.Set(WestWoodlandsDungeonQuestStage.Completed);
                aisling.SendOrangeBarMessage("You've cleared Lost Woodlands! An escape portal opens.");
            }

            var player = aislings.FirstOrDefault(x => x.Trackers.Enums.HasValue(WestWoodlandsDungeonQuestStage.Completed));

            if (player == null)
                return;
            
            var portalSpawn = new Rectangle(player, 6, 6);
            var outline = portalSpawn.GetOutline().ToList();
            Point point;

            do
                point = outline.PickRandom();
            while (!Subject.MapInstance.IsWalkable(point, player.Type));
                
            var reactortile = ReactorTileFactory.Create("wwdungeonescapeportal", Subject.MapInstance, Point.From(point));
                
            Subject.MapInstance.SimpleAdd(reactortile);
        }
    }
}