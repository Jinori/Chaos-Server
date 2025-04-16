using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Services.Servers.Options;
using Chaos.Time;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Easter;

public sealed class MrHoppsDeathScript : MonsterScriptBase
{
    public MrHoppsDeathScript(Monster subject)
        : base(subject)
        => ExperienceDistributionScript = subject.Template.TemplateKey.ContainsI("98")
            ? NonMasterScalingExperienceDistributionScript.Create()
            : DefaultExperienceDistributionScript.Create();

    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    private void DistributeLootAndExperience(Aisling rewardTarget, Aisling[]? rewardTargets)
    {
        if ((rewardTargets == null) || (rewardTargets.Length == 0))
            return;

        switch (rewardTarget.Group?.LootOption)
        {
            case Group.GroupLootOption.Default:
                HandleLootDrop(rewardTargets, true);

                break;

            case Group.GroupLootOption.Random:
                // Ensure only members on the same map as the Subject (monster) receive loot
                rewardTarget.Group.DistributeRandomized(Subject.Items, Subject);

                // Ensure only members on the same map as the Subject receive gold
                rewardTarget.Group.DistributeEvenGold(Subject.Gold, Subject, false);

                break;

            case Group.GroupLootOption.MasterLooter:
                HandleLootDrop(rewardTargets, lockToLeader: rewardTarget.Group.Leader);

                break;
        }
    }

    private void DropLootAndExperience(Aisling[]? rewardTargets) => HandleLootDrop(rewardTargets, rewardTargets != null);

    private Aisling? GetHighestContributor()
        => Subject.Contribution
                  .OrderByDescending(kvp => kvp.Value)
                  .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                  .FirstOrDefault(a => a != null);

    private Aisling? GetLastDamagerFromTrap()
    {
        var lastDamage = Subject.Trackers.LastDamagedBy;

        var trap = Subject.MapInstance
                          .GetDistinctReactorsAtPoint(Subject)
                          .Where(x => x.Script.Is<TrapScript>() && (lastDamage != null) && x.WithinRange(lastDamage));

        return trap.Any() ? lastDamage as Aisling : null;
    }

    private static Aisling[] GetRewardTargets(Aisling rewardTarget)
    {
        IEnumerable<Aisling> groupOrSingle = rewardTarget.Group != null
            ? rewardTarget.Group
            : new[]
            {
                rewardTarget
            };

        return groupOrSingle.ThatAreWithinRange(rewardTarget)
                            .ToArray();
    }

    private void HandleLootDrop(Aisling[]? rewardTargets, bool lockToTargets = false, Aisling? lockToLeader = null)
    {
        var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
        var droppedItems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        if (WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
        {
            var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

            if (droppedGold && (money != null))
            {
                if (lockToLeader != null)
                    money.LockToAislings(lockSecs, lockToLeader);
                else if (lockToTargets && (rewardTargets != null))
                    money.LockToAislings(lockSecs, rewardTargets);
            }

            if (droppedItems && (groundItems != null))
                foreach (var groundItem in groundItems)
                    if (lockToLeader != null)
                        groundItem.LockToAislings(lockSecs, lockToLeader);
                    else if (lockToTargets && (rewardTargets != null))
                        groundItem.LockToAislings(lockSecs, rewardTargets);
        }
    }

    private void HandleMapLootAndExperience()
    {
        var playersOnEventMap = Map.GetEntities<Aisling>()
                                   .Where(x => !x.IsGodModeEnabled() && x.IsAlive)
                                   .ToList();

        if (!playersOnEventMap.Any())
            return;

        foreach (var aisling in playersOnEventMap)
        {
            ExperienceDistributionScript.GiveExp(aisling, Subject.Experience);

            aisling.Legend.AddOrAccumulate(
                new LegendMark(
                    "Defeated Mr.Hopps",
                    "mrhoppsfloppyfields",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));
        }

        Subject.Items.Clear();
        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        // Now randomly assign loot from the regenerated loot table
        foreach (var item in Subject.Items)
        {
            var randomMember = playersOnEventMap.PickRandom();
            randomMember.GiveItemOrSendToBank(item);
            randomMember.SendOrangeBarMessage($"You received {item.DisplayName} from {Subject.Name}");
        }
    }

    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var rewardTarget = GetHighestContributor() ?? GetLastDamagerFromTrap();

        var rewardTargets = rewardTarget != null ? GetRewardTargets(rewardTarget) : null;

        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        if (rewardTarget is { Group: not null })
            DistributeLootAndExperience(rewardTarget, rewardTargets);
        else
            DropLootAndExperience(rewardTargets);

        HandleMapLootAndExperience();
    }
}