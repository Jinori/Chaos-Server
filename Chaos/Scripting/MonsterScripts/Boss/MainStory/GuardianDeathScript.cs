using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory;

// ReSharper disable once ClassCanBeSealed.Global
public class GuardianDeathScript : MonsterScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public GuardianDeathScript(Monster subject, ISimpleCache simpleCache, IItemFactory itemFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
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

            foreach (var member in Subject.MapInstance
                                          .GetEntities<Aisling>()
                                          .Where(
                                              x => x.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1)
                                                   || x.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2)
                                                   || x.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3)
                                                   || x.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4))
                                          .ToList())
            {
                if (Subject.Name == "Earth Guardian")
                    if (!member.Inventory.Contains("Earth Artifact"))
                    {
                        var artifact = ItemFactory.Create("earthartifact");
                        member.GiveItemOrSendToBank(artifact);
                        member.SendOrangeBarMessage("You pull the Earth Artifact from the mud.");
                    }

                if (Subject.Name == "Flame Guardian")
                    if (!member.Inventory.Contains("Fire Artifact"))
                    {
                        var artifact = ItemFactory.Create("fireartifact");
                        member.GiveItemOrSendToBank(artifact);
                        member.SendOrangeBarMessage("You pull the Fire Artifact from its ashes.");
                    }

                if (Subject.Name == "Wind Guardian")
                    if (!member.Inventory.Contains("Wind Artifact"))
                    {
                        var artifact = ItemFactory.Create("windartifact");
                        member.GiveItemOrSendToBank(artifact);
                        member.SendOrangeBarMessage("You pull the Wind Artifact from the ground.");
                    }

                if (Subject.Name == "Sea Guardian")
                    if (!member.Inventory.Contains("Sea Artifact"))
                    {
                        var artifact = ItemFactory.Create("seaartifact");
                        member.GiveItemOrSendToBank(artifact);
                        member.SendOrangeBarMessage("You pull the Sea Artifact from the sand.");
                    }
            }
        }
    }
}