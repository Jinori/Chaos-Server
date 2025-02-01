using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Quests.BountyBoard;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Kill_Counters;

public class BountyBoardCounterScript(Monster subject) : ConfigurableMonsterScriptBase(subject)
{
    #region ScriptVars
    public string? BountyMonsterKey { get; init; } // ✅ Matches monsterKey in dictionary
    #endregion

    public override void OnDeath()
    {
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Subject.MapInstance.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null);

        if (rewardTarget is null)
            return;

        var rewardTargets = rewardTarget.Group != null
            ? rewardTarget.Group
                          .ThatAreWithinRange(rewardTarget)
                          .ToArray()
            : new[]
            {
                rewardTarget
            };

        foreach (var aisling in rewardTargets)
        {
            var bountyTypes = new[]
            {
                typeof(BountyBoardKill1),
                typeof(BountyBoardKill2),
                typeof(BountyBoardKill3)
            };

            // ✅ Loop through all three bounty types
            foreach (var bountyType in bountyTypes)
            {
                var bountyDictionary = BountyBoardDialogScript.GetBountyDictionary(bountyType);

                var monsterKey = BountyMonsterKey?.ToLowerInvariant();

                // ✅ Ensure the player accepted this bounty
                if (string.IsNullOrEmpty(monsterKey) || !bountyDictionary.Values.Any(b => b.MonsterKey == monsterKey))
                    continue;

                if (!PlayerHasBounty(aisling, monsterKey))
                    continue;

                // ✅ Retrieve the correct bounty based on difficulty flag
                var bountyData = bountyDictionary.Values.FirstOrDefault(
                    b => (b.MonsterKey == monsterKey) && aisling.Trackers.Flags.HasFlag(b.DifficultyFlag));

                if (bountyData == default)
                {
                    Console.WriteLine($"❌ ERROR: No bounty found for monsterKey '{monsterKey}' with matching difficulty.");

                    continue;
                }

                (var monster, var killRequirement, var killEnum, _, _) = bountyData;

                // ✅ Ensure this is the correct bounty the player has
                if (!aisling.Trackers.Enums.TryGetValue(bountyType, out var activeBounty) || !killEnum.Equals(activeBounty))
                    continue;

                // ✅ Ensure the kill is counted
                if (!aisling.Trackers.Counters.TryGetValue(monster, out _))
                    aisling.Trackers.Counters.Set(monster, 0);

                if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(monster, killRequirement))
                {
                    aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}.");

                    break; // ✅ Exit after finding a match to avoid duplicate processing
                }

                var newCount = aisling.Trackers.Counters.AddOrIncrement(monster);
                aisling.Client.SendServerMessage(ServerMessageType.PersistentMessage, $"{newCount} - {Subject.Template.Name}");

                break; // ✅ Once a match is found, stop checking other bounty types
            }
        }
    }

    private bool PlayerHasBounty(Aisling aisling, string monsterKey)
    {
        var bountyTypes = new[]
        {
            typeof(BountyBoardKill1),
            typeof(BountyBoardKill2),
            typeof(BountyBoardKill3)
        };

        foreach (var bountyType in bountyTypes)
        {
            var bountyDict = BountyBoardDialogScript.GetBountyDictionary(bountyType);

            foreach (var bountyData in bountyDict.Values)
            {
                // ✅ Check if monsterKey matches
                if (bountyData.MonsterKey != monsterKey)
                    continue;

                // ✅ Check if player has this bounty
                if (!aisling.Trackers.Enums.TryGetValue(bountyType, out var activeBounty) || !bountyData.KillEnum.Equals(activeBounty))
                    continue;

                // ✅ Check if player has the correct difficulty flag
                if (!aisling.Trackers.Flags.HasFlag(bountyData.DifficultyFlag))
                    continue;

                // ✅ If all conditions match, the player has the correct bounty
                return true;
            }
        }

        return false;
    }
}