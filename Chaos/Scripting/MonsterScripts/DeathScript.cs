using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts
{
    public class DeathScript(Monster subject) : MonsterScriptBase(subject)
    {
        private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

        public override void OnDeath()
        {
            if (!Map.RemoveEntity(Subject)) return;

            var rewardTarget = GetHighestContributor() ?? GetLastDamagerFromTrap();

            var rewardTargets = rewardTarget != null
                ? GetRewardTargets(rewardTarget)
                : null;

            Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

            if (rewardTarget != null && rewardTarget.Group != null)
            {
                DistributeLootAndExperience(rewardTarget, rewardTargets);
            }
            else
            {
                DropLootAndExperience(rewardTargets);
            }
        }

        private Aisling? GetHighestContributor()
        {
            return Subject.Contribution
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                .FirstOrDefault(a => a != null);
        }

        private Aisling? GetLastDamagerFromTrap()
        {
            var lastDamage = Subject.Trackers.LastDamagedBy;
            var trap = Subject.MapInstance.GetDistinctReactorsAtPoint(Subject)
                .Where(x => x.Script.Is<TrapScript>() && lastDamage != null && x.WithinRange(lastDamage));

            return trap.Any() ? lastDamage as Aisling : null;
        }

        private static Aisling[] GetRewardTargets(Aisling rewardTarget)
        {
            IEnumerable<Aisling> groupOrSingle = rewardTarget.Group != null 
                ? rewardTarget.Group.Members 
                : new[] { rewardTarget };

            return groupOrSingle
                .ThatAreWithinRange(rewardTarget)
                .ToArray();
        }




        private void DistributeLootAndExperience(Aisling rewardTarget, Aisling[]? rewardTargets)
        {
            if (rewardTargets == null || rewardTargets.Length == 0) return;
    
            switch (rewardTarget.Group?.LootOption)
            {
                case Group.GroupLootOption.Default:
                    HandleLootDrop(rewardTargets, lockToTargets: true);
                    ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
                    break;

                case Group.GroupLootOption.Random:
                    rewardTarget.Group.DistributeRandomized(Subject.Items);
                    rewardTarget.Group.DistributeEvenGold(Subject.Gold);
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
            HandleLootDrop(rewardTargets, lockToTargets: rewardTargets != null);
            if (rewardTargets != null)
            {
                ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
            }
        }

        private void HandleLootDrop(Aisling[]? rewardTargets, bool lockToTargets = false, Aisling? lockToLeader = null)
        {
            var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
            var droppedItems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

            if (WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
            {
                var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

                if (droppedGold && money != null)
                {
                    if (lockToLeader != null)
                    {
                        money.LockToAislings(lockSecs, lockToLeader);
                    }
                    else if (lockToTargets && rewardTargets != null)
                    {
                        money.LockToAislings(lockSecs, rewardTargets);
                    }
                }

                if (droppedItems && groundItems != null)
                {
                    foreach (var groundItem in groundItems)
                    {
                        if (lockToLeader != null)
                        {
                            groundItem.LockToAislings(lockSecs, lockToLeader);
                        }
                        else if (lockToTargets && rewardTargets != null)
                        {
                            groundItem.LockToAislings(lockSecs, rewardTargets);
                        }
                    }
                }
            }
        }
    }
}
