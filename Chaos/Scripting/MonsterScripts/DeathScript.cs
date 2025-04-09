#region
using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Services.Servers.Options;
#endregion

namespace Chaos.Scripting.MonsterScripts;

public class DeathScript(Monster subject) : MonsterScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private void DistributeLootAndExperience(Aisling rewardTarget, Aisling[]? rewardTargets)
    {
        if ((rewardTargets == null) || (rewardTargets.Length == 0))
            return;

        switch (rewardTarget.Group?.LootOption)
        {
            case Group.GroupLootOption.Default:
                HandleLootDrop(rewardTargets, true);
                ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);

                break;

            case Group.GroupLootOption.Random:
                // Ensure only members on the same map as the Subject (monster) receive loot
                rewardTarget.Group.DistributeRandomized(Subject.Items, Subject);

                var hasDropBoost = rewardTarget.Group.Any(a => a.Effects.Contains("dropboost"));
                var goldAmount = Subject.Gold;

                // Apply gold boost if it's `true`
                if (hasDropBoost)
                    goldAmount = (int)(goldAmount * 1.25);

                // Ensure only members on the same map as the Subject receive gold
                rewardTarget.Group.DistributeEvenGold(goldAmount, Subject, hasDropBoost);

                // Distribute experience only to members on the same map as the monster
                ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);

                break;

            case Group.GroupLootOption.MasterLooter:
                HandleLootDrop(rewardTargets, lockToLeader: rewardTarget.Group.Leader);
                ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);

                break;
        }
    }

    private void DropLootAndExperience(Aisling[]? rewardTargets)
    {
        HandleLootDrop(rewardTargets, rewardTargets != null);

        if (rewardTargets != null)
            ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
    }

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
        var hasGoldBoost = rewardTargets?.Any(a => a.Effects.Contains("DropBoost"));
        var originalGoldAmount = Subject.Gold;

        // Apply gold boost if it's active
        if (hasGoldBoost == true)
            originalGoldAmount = (int)(originalGoldAmount * 1.25);

        // Determine how much to tax and adjust final drop amount
        var taxAmount = 0;
        var finalGoldAmount = originalGoldAmount;

        if (rewardTargets is { Length: > 0 })
        {
            // Distribute tax equally among members with guilds that have a tax rate
            foreach (var aisling in rewardTargets)
            {
                if ((aisling.Guild != null) && (aisling.Guild.TaxRatePercent > 0))
                {
                    var share = originalGoldAmount / rewardTargets.Length;
                    var taxShare = (int)Math.Round(share * (aisling.Guild.TaxRatePercent / 100.0));

                    if (taxShare > 0)
                        aisling.Guild.Bank.AddGold((uint)taxShare);

                    taxAmount += taxShare;
                }
            }

            finalGoldAmount = Math.Max(0, originalGoldAmount - taxAmount);
        }

        // Attempt to drop the adjusted amount of gold
        var droppedGold = Subject.TryDropGold(Subject, finalGoldAmount, out var money);
        var droppedItems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        // Lock gold/items to players if configured
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
            {
                foreach (var groundItem in groundItems)
                {
                    if (lockToLeader != null)
                        groundItem.LockToAislings(lockSecs, lockToLeader);
                    else if (lockToTargets && (rewardTargets != null))
                        groundItem.LockToAislings(lockSecs, rewardTargets);
                }
            }
        }
    }


    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var rewardTarget = GetHighestContributor() ?? GetLastDamagerFromTrap();

        var rewardTargets = rewardTarget != null ? GetRewardTargets(rewardTarget) : null;

        var serendaelBuff = (rewardTargets != null) && rewardTargets.Any(x => x.Effects.Contains("dropboost"));

        Subject.Items.AddRange(Subject.LootTable.GenerateLoot(serendaelBuff));

        if (rewardTarget is { Group: not null })
            DistributeLootAndExperience(rewardTarget, rewardTargets);
        else
            DropLootAndExperience(rewardTargets);
    }
}