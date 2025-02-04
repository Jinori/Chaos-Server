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
    public string? BountyMonsterKey { get; init; } // ✅ Matches monsterKey in the dictionaries
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
            // We'll check each of the 3 bounty slots.
            var bountyTypes = new[]
            {
                typeof(BountyBoardKill1),
                typeof(BountyBoardKill2),
                typeof(BountyBoardKill3)
            };

            foreach (var bountyType in bountyTypes)
            {
                var bountyDictionary = BountyBoardDialogScript.GetBountyDictionary(bountyType);
                var monsterKey = BountyMonsterKey?.ToLowerInvariant();

                // 1) If no monsterKey or dictionary doesn't contain this monster, skip
                if (string.IsNullOrEmpty(monsterKey) || !bountyDictionary.Values.Any(b => b.MonsterKey == monsterKey))
                    continue;

                // 2) Figure out which difficulty flag is actually set for this monster
                //    e.g. Easy1, Medium1, Hard1, etc.  We'll find that from the dictionary.
                var bountyData = bountyDictionary.Values.FirstOrDefault(
                    b => (b.MonsterKey == monsterKey) && aisling.Trackers.Flags.HasFlag(b.DifficultyFlag));

                // If the user doesn't have, e.g., "Easy2" (or whichever difficulty is needed),
                // then bountyData will be default.
                if (bountyData.Equals(default))

                    // Not a matching difficulty for this bountyType => skip.
                    continue;

                // 3) Now check if the user is actually on this exact slot/bounty
                (var monster, var killRequirement, var killEnum, _, _) = bountyData;

                if (!aisling.Trackers.Enums.TryGetValue(bountyType, out var activeBounty) || !killEnum.Equals(activeBounty))

                    // This means, e.g., the dictionary is BountyBoardKill1, but the user
                    // might really have it in BountyBoardKill2 => skip.
                    continue;

                // 4) We now have a valid slot and difficulty. Make sure we have a counter for it:
                if (!aisling.Trackers.Counters.TryGetValue(monster, out _))
                    aisling.Trackers.Counters.Set(monster, 0);

                // 5) Check if they're already at or above the kill requirement
                if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(monster, killRequirement))
                {
                    aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}!");

                    // We break from the bountyType loop so we don't double-increment
                    break;
                }

                // 6) Otherwise, increment kills and inform the user
                var newCount = aisling.Trackers.Counters.AddOrIncrement(monster);
                aisling.Client.SendServerMessage(ServerMessageType.PersistentMessage, $"{newCount} - {Subject.Template.Name}");

                // 7) Break once we've found the correct match and incremented
                break;
            }
        }
    }
}