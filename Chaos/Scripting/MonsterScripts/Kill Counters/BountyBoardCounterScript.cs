using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Kill_Counters;

public class BountyBoardCounterScript(Monster subject) : ConfigurableMonsterScriptBase(subject)
{
    #region ScriptVars
    public string? BountyMonsterKey { get; init; } // ✅ Matches monsterKey in the dictionaries
    #endregion

    public IEnumerable<BountyDetails> GetCurrentBounties(Aisling source)
    {
        if (source.Trackers.Flags.TryGetFlag(typeof(BountyQuestFlags1), out var flags1))
            foreach (var bounty in BountyBoardQuests.PossibleQuestDetails.Where(bounty => flags1.HasFlag(bounty.BountyQuestFlag)))
                yield return bounty;

        if (source.Trackers.Flags.TryGetFlag(typeof(BountyQuestFlags2), out var flags2))
            foreach (var bounty in BountyBoardQuests.PossibleQuestDetails.Where(bounty => flags2.HasFlag(bounty.BountyQuestFlag)))
                yield return bounty;

        if (source.Trackers.Flags.TryGetFlag(typeof(BountyQuestFlags3), out var flags3))
            foreach (var bounty in BountyBoardQuests.PossibleQuestDetails.Where(bounty => flags3.HasFlag(bounty.BountyQuestFlag)))
                yield return bounty;
    }

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
            var currentBounties = GetCurrentBounties(aisling)
                .ToList();

            if (!currentBounties.Any())
                return;

            foreach (var currentbounty in currentBounties)
            {
                if (currentbounty.MonsterTemplateKey != BountyMonsterKey)
                    continue;

                // 4) We now have a valid slot and difficulty. Make sure we have a counter for it:
                if (!aisling.Trackers.Counters.TryGetValue(currentbounty.MonsterTemplateKey, out _))
                    aisling.Trackers.Counters.Set(currentbounty.MonsterTemplateKey, 0);

                // 5) Check if they're already at or above the kill requirement
                if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo(currentbounty.MonsterTemplateKey, currentbounty.KillRequirement))
                {
                    aisling.SendOrangeBarMessage($"You've killed enough {Subject.Template.Name}!");

                    // We break from the bountyType loop so we don't double-increment
                    break;
                }

                // 6) Otherwise, increment kills and inform the user
                var newCount = aisling.Trackers.Counters.AddOrIncrement(currentbounty.MonsterTemplateKey);
                aisling.Client.SendServerMessage(ServerMessageType.PersistentMessage, $"{newCount} - {Subject.Template.Name}");

                // 7) Break once we've found the correct match and incremented
                break;
            }
        }
    }
}