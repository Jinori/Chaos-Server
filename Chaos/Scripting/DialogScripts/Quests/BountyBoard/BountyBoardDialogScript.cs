using System.Text;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests.BountyBoard;

public class BountyBoardDialogScript(Dialog subject, ILogger<BountyBoardDialogScript> logger) : DialogScriptBase(subject)
{
    private static readonly Random Random = new();

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private void AbandonBounty(Aisling source, string bountyName)
    {
        // Safely try to retrieve the bounty data from any of the three dictionaries
        (var monsterKey, _, var killEnum, var originalFlag, _) = GetBountyFromAnyDictionary(bountyName);

        // Check each slot to see which one actually holds this bounty.
        // Remove the bounty from that slot, its counter, and the corresponding difficulty flag.
        if (source.Trackers.Enums.TryGetValue(out BountyBoardKill1 slot1Bounty) && slot1Bounty.Equals(killEnum))
        {
            // Clear the enum from Slot 1
            source.Trackers.Enums.Remove(typeof(BountyBoardKill1));

            // Remove the monster kill counter
            source.Trackers.Counters.Remove(monsterKey, out _);

            // Convert originalFlag into the correct suffix for slot "1"
            var difficultyFlag = GetMatchingDifficultyFlag(originalFlag, 1);
            source.Trackers.Flags.RemoveFlag(typeof(BountyBoardFlags), difficultyFlag);
        } else if (source.Trackers.Enums.TryGetValue(out BountyBoardKill2 slot2Bounty) && slot2Bounty.Equals(killEnum))
        {
            source.Trackers.Enums.Remove(typeof(BountyBoardKill2));
            source.Trackers.Counters.Remove(monsterKey, out _);

            var difficultyFlag = GetMatchingDifficultyFlag(originalFlag, 2);
            source.Trackers.Flags.RemoveFlag(typeof(BountyBoardFlags), difficultyFlag);
        } else if (source.Trackers.Enums.TryGetValue(out BountyBoardKill3 slot3Bounty) && slot3Bounty.Equals(killEnum))
        {
            source.Trackers.Enums.Remove(typeof(BountyBoardKill3));
            source.Trackers.Counters.Remove(monsterKey, out _);

            var difficultyFlag = GetMatchingDifficultyFlag(originalFlag, 3);
            source.Trackers.Flags.RemoveFlag(typeof(BountyBoardFlags), difficultyFlag);
        }
    }

    private void CompleteBounty<T>(Aisling source, T bounty, Type bountyEnumType) where T: Enum
    {
        var bountyDictionary = GetBountyDictionary(bountyEnumType);
        var bountyEntry = bountyDictionary.FirstOrDefault(x => x.Value.KillEnum.Equals(bounty));

        if (bountyEntry.Key == null)
        {
            Subject.Reply(source, "Error: Unable to find the corresponding bounty in the system.", "bountyboard_initial");

            return;
        }

        (var monsterKey, var killRequirement, _, var difficultyFlag, _) = bountyEntry.Value;

        // ✅ Use `BountyMonsterKey` instead of the default counter key
        if (!source.Trackers.Counters.TryGetValue(monsterKey, out var currentKills) || (currentKills < killRequirement))
        {
            Subject.Reply(source, $"You have not completed the bounty for {bountyEntry.Key} yet.", "bountyboard_initial");

            return;
        }

        // ✅ Grant rewards
        GrantBountyRewards(source, bountyEntry.Key, killRequirement);

        // ✅ Remove the bounty slot
        source.Trackers.Enums.Remove(bountyEnumType);

        // ✅ Remove the kill counter
        source.Trackers.Counters.Remove(monsterKey, out _);

        // ✅ Remove only the specific difficulty flag
        source.Trackers.Flags.RemoveFlag(typeof(BountyBoardFlags), difficultyFlag);

        Subject.Reply(source, $"You have completed the bounty: {bountyEntry.Key}! Well done.", "bountyboard_initial");
    }

    private List<string> GetAllBounties()
        => GetBountyDictionary(typeof(BountyBoardKill1))
           .Keys
           .Concat(
               GetBountyDictionary(typeof(BountyBoardKill2))
                   .Keys)
           .Concat(
               GetBountyDictionary(typeof(BountyBoardKill3))
                   .Keys)
           .ToList();

    public static Dictionary<string, (string MonsterKey, int KillRequirement, Enum KillEnum, BountyBoardFlags DifficultyFlag,
        BountyBoardOptions BountyOption)> GetBountyDictionary(Type bountyEnumType)
        => bountyEnumType switch
        {
            _ when bountyEnumType == typeof(BountyBoardKill1) => BountyBoardDictionary.BountyOptions1.ToDictionary(
                x => x.Key,
                x => (x.Value.MonsterKey, x.Value.KillRequirement, (Enum)x.Value.KillEnum, x.Value.DifficultyFlag, x.Value.BountyOption)),
            _ when bountyEnumType == typeof(BountyBoardKill2) => BountyBoardDictionary.BountyOptions2.ToDictionary(
                x => x.Key,
                x => (x.Value.MonsterKey, x.Value.KillRequirement, (Enum)x.Value.KillEnum, x.Value.DifficultyFlag, x.Value.BountyOption)),
            _ when bountyEnumType == typeof(BountyBoardKill3) => BountyBoardDictionary.BountyOptions3.ToDictionary(
                x => x.Key,
                x => (x.Value.MonsterKey, x.Value.KillRequirement, (Enum)x.Value.KillEnum, x.Value.DifficultyFlag, x.Value.BountyOption)),
            _ => throw new InvalidOperationException("Invalid bounty enum type.")
        };

    private (string, int, Enum, BountyBoardFlags, BountyBoardOptions) GetBountyFromAnyDictionary(string bounty)
        => GetBountyDictionary(typeof(BountyBoardKill1))
            .TryGetValue(bounty, out var bountyData)
            ? bountyData
            : GetBountyDictionary(typeof(BountyBoardKill2))
                .TryGetValue(bounty, out bountyData)
                ? bountyData
                : GetBountyDictionary(typeof(BountyBoardKill3))
                    .TryGetValue(bounty, out bountyData)
                    ? bountyData
                    : throw new KeyNotFoundException($"Bounty '{bounty}' not found in any dictionary.");

    private BountyBoardFlags GetMatchingDifficultyFlag(BountyBoardFlags originalFlag, int slot)
    {
        var flagName = originalFlag.ToString();

        if (flagName.EndsWith('1'))
            return Enum.Parse<BountyBoardFlags>(flagName.Replace("1", slot.ToString()));

        if (flagName.EndsWith('2'))
            return Enum.Parse<BountyBoardFlags>(flagName.Replace("2", slot.ToString()));

        if (flagName.EndsWith('3'))
            return Enum.Parse<BountyBoardFlags>(flagName.Replace("3", slot.ToString()));

        return originalFlag; // Default to original if no match
    }

    private void GrantBountyRewards(Aisling source, string bountyName, int killRequirement)
    {
        var baseExp = 500; // Base XP per bounty
        var goldReward = 1000; // Base Gold Reward

        // Adjust reward based on difficulty
        if (killRequirement == 100)
        {
            baseExp = 500;
            goldReward = 1000;
        } else if (killRequirement == 500)
        {
            baseExp = 2500;
            goldReward = 5000;
        } else if (killRequirement == 1000)
        {
            baseExp = 5000;
            goldReward = 10000;
        }

        // Grant EXP and Gold
        ExperienceDistributionScript.GiveExp(source, baseExp);
        source.TryGiveGold(goldReward);

        Subject.Reply(source, $"You received {baseExp} experience and {goldReward} gold for completing {bountyName}.", "bountyboard_initial");
    }

    private bool HasBountyCompleted(Aisling source, Type bountyEnumType)
    {
        if (!source.Trackers.Enums.TryGetValue(bountyEnumType, out var activeBounty) || activeBounty.Equals(default(Enum)))
            return false; // ✅ No active bounty in this slot, so no completion check needed.

        var bountyDictionary = GetBountyDictionary(bountyEnumType);

        foreach ((_, (var monsterKey, var killRequirement, var killEnum, var difficultyFlag, _)) in bountyDictionary)
        {
            if (string.IsNullOrEmpty(monsterKey) || !killEnum.Equals(activeBounty))
                continue; // ✅ Ensure we only check the active bounty

            // ✅ Validate Kill Requirement Based on Difficulty
            if (source.Trackers.Flags.HasFlag(difficultyFlag)
                && source.Trackers.Counters.TryGetValue(monsterKey, out var currentKills)
                && (currentKills >= killRequirement))
                return true;
        }

        return false;
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage1 = source.Trackers.Enums.TryGetValue(out BountyBoardKill1 _);
        var hasStage2 = source.Trackers.Enums.TryGetValue(out BountyBoardKill2 _);
        var hasStage3 = source.Trackers.Enums.TryGetValue(out BountyBoardKill3 _);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bountyboard_initial":
            {
                if (!source.UserStatSheet.Master)
                {
                    Subject.Reply(source, "The bounty board doesn't make any sense to you. (Master Only)");

                    return;
                }

                Subject.Options.Add(
                    new DialogOption
                    {
                        DialogKey = "bountyboard_viewactive",
                        OptionText = "View Active Bounties"
                    });

                Subject.Options.Insert(
                    0,
                    new DialogOption
                    {
                        DialogKey = "bountyboard_questinitial",
                        OptionText = "Browse Posted Bounties"
                    });

                if (hasStage1 && HasBountyCompleted(source, typeof(BountyBoardKill1)))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_turnin1",
                            OptionText = "Turn in Bounty"
                        });

                if (hasStage2 && HasBountyCompleted(source, typeof(BountyBoardKill2)))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_turnin2",
                            OptionText = "Turn in Bounty"
                        });

                if (hasStage3 && HasBountyCompleted(source, typeof(BountyBoardKill3)))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_turnin3",
                            OptionText = "Turn in Bounty"
                        });

                // NEW: If any slot is active, allow "Abandon a Bounty"
                if (hasStage1 || hasStage2 || hasStage3)

                    // Add "Abandon a Bounty" option
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_abandon",
                            OptionText = "Abandon a Bounty"
                        });

                break;
            }

            case "bountyboard_viewactive":
            {
                var sb = new StringBuilder();

                // SLOT 1
                if (source.Trackers.Enums.TryGetValue(out BountyBoardKill1 slot1Bounty) && !slot1Bounty.Equals(default))
                {
                    var dict1 = GetBountyDictionary(typeof(BountyBoardKill1));

                    BountyBoardFlags? slotFlag = null;

                    if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Epic1))
                        slotFlag = BountyBoardFlags.Epic1;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Hard1))
                        slotFlag = BountyBoardFlags.Hard1;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Medium1))
                        slotFlag = BountyBoardFlags.Medium1;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Easy1))
                        slotFlag = BountyBoardFlags.Easy1;

                    if (slotFlag.HasValue)
                    {
                        var entry = dict1.FirstOrDefault(
                            x => x.Value.KillEnum.Equals(slot1Bounty) && (x.Value.DifficultyFlag == slotFlag.Value));

                        if (!string.IsNullOrEmpty(entry.Key))
                        {
                            (var monsterKey, var required, _, _, _) = entry.Value;

                            source.Trackers.Counters.TryGetValue(monsterKey, out var killsSoFar);
                            killsSoFar = Math.Max(0, killsSoFar);

                            sb.Append($"{entry.Key} - {killsSoFar}/{required}\n");
                        }
                    }
                }

                // SLOT 2
                if (source.Trackers.Enums.TryGetValue(out BountyBoardKill2 slot2Bounty) && !slot2Bounty.Equals(default))
                {
                    var dict2 = GetBountyDictionary(typeof(BountyBoardKill2));

                    BountyBoardFlags? slotFlag = null;

                    if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Epic2))
                        slotFlag = BountyBoardFlags.Epic2;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Hard2))
                        slotFlag = BountyBoardFlags.Hard2;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Medium2))
                        slotFlag = BountyBoardFlags.Medium2;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Easy2))
                        slotFlag = BountyBoardFlags.Easy2;

                    if (slotFlag.HasValue)
                    {
                        var entry = dict2.FirstOrDefault(
                            x => x.Value.KillEnum.Equals(slot2Bounty) && (x.Value.DifficultyFlag == slotFlag.Value));

                        if (!string.IsNullOrEmpty(entry.Key))
                        {
                            (var monsterKey, var required, _, _, _) = entry.Value;

                            source.Trackers.Counters.TryGetValue(monsterKey, out var killsSoFar);
                            killsSoFar = Math.Max(0, killsSoFar);

                            sb.Append($"{entry.Key} - {killsSoFar}/{required}\n");
                        }
                    }
                }

                // SLOT 3
                if (source.Trackers.Enums.TryGetValue(out BountyBoardKill3 slot3Bounty) && !slot3Bounty.Equals(default))
                {
                    var dict3 = GetBountyDictionary(typeof(BountyBoardKill3));

                    BountyBoardFlags? slotFlag = null;

                    if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Epic3))
                        slotFlag = BountyBoardFlags.Epic3;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Hard3))
                        slotFlag = BountyBoardFlags.Hard3;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Medium3))
                        slotFlag = BountyBoardFlags.Medium3;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Easy3))
                        slotFlag = BountyBoardFlags.Easy3;

                    if (slotFlag.HasValue)
                    {
                        var entry = dict3.FirstOrDefault(
                            x => x.Value.KillEnum.Equals(slot3Bounty) && (x.Value.DifficultyFlag == slotFlag.Value));

                        if (!string.IsNullOrEmpty(entry.Key))
                        {
                            (var monsterKey, var required, _, _, _) = entry.Value;

                            source.Trackers.Counters.TryGetValue(monsterKey, out var killsSoFar);
                            killsSoFar = Math.Max(0, killsSoFar);

                            sb.Append($"{entry.Key} - {killsSoFar}/{required}");
                        }
                    }
                }

                // If sb is empty, the user has no active bounties
                if (sb.Length == 0)
                    Subject.Reply(source, "You have no active bounties right now.", "bountyboard_initial");
                else
                    Subject.Reply(source, "Your Current Bounties:\n" + sb, "bountyboard_initial");

                break;
            }

            case "bountyboard_abandon":
            {
                // Collect names of the bounties the player is actually on
                var activeBounties = new List<string>();

                if (hasStage1 && source.Trackers.Enums.TryGetValue(out BountyBoardKill1 bounty1))
                {
                    var dict = GetBountyDictionary(typeof(BountyBoardKill1));

                    // Figure out which difficulty is set for slot 1
                    BountyBoardFlags? slotFlag = null;

                    if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Epic1))
                        slotFlag = BountyBoardFlags.Epic1;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Hard1))
                        slotFlag = BountyBoardFlags.Hard1;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Medium1))
                        slotFlag = BountyBoardFlags.Medium1;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Easy1))
                        slotFlag = BountyBoardFlags.Easy1;

                    if (slotFlag.HasValue)
                    {
                        // Now find the dictionary entry that has BOTH the killEnum + the correct difficulty
                        var entry
                            = dict.FirstOrDefault(x => x.Value.KillEnum.Equals(bounty1) && (x.Value.DifficultyFlag == slotFlag.Value));

                        if (!string.IsNullOrEmpty(entry.Key))
                            activeBounties.Add(entry.Key);
                    }
                }

                if (hasStage2 && source.Trackers.Enums.TryGetValue(out BountyBoardKill2 bounty2))
                {
                    var dict = GetBountyDictionary(typeof(BountyBoardKill2));

                    // Figure out which difficulty is set for slot 2
                    BountyBoardFlags? slotFlag = null;

                    if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Epic2))
                        slotFlag = BountyBoardFlags.Epic2;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Hard2))
                        slotFlag = BountyBoardFlags.Hard2;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Medium2))
                        slotFlag = BountyBoardFlags.Medium2;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Easy2))
                        slotFlag = BountyBoardFlags.Easy2;

                    if (slotFlag.HasValue)
                    {
                        // Now find the dictionary entry that has BOTH the killEnum + the correct difficulty
                        var entry
                            = dict.FirstOrDefault(x => x.Value.KillEnum.Equals(bounty2) && (x.Value.DifficultyFlag == slotFlag.Value));

                        if (!string.IsNullOrEmpty(entry.Key))
                            activeBounties.Add(entry.Key);
                    }
                }

                if (hasStage3 && source.Trackers.Enums.TryGetValue(out BountyBoardKill3 bounty3))
                {
                    var dict = GetBountyDictionary(typeof(BountyBoardKill3));

                    // Figure out which difficulty is set for slot 3
                    BountyBoardFlags? slotFlag = null;

                    if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Epic3))
                        slotFlag = BountyBoardFlags.Epic3;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Hard3))
                        slotFlag = BountyBoardFlags.Hard3;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Medium3))
                        slotFlag = BountyBoardFlags.Medium3;
                    else if (source.Trackers.Flags.HasFlag(BountyBoardFlags.Easy3))
                        slotFlag = BountyBoardFlags.Easy3;

                    if (slotFlag.HasValue)
                    {
                        // Now find the dictionary entry that has BOTH the killEnum + the correct difficulty
                        var entry
                            = dict.FirstOrDefault(x => x.Value.KillEnum.Equals(bounty3) && (x.Value.DifficultyFlag == slotFlag.Value));

                        if (!string.IsNullOrEmpty(entry.Key))
                            activeBounties.Add(entry.Key);
                    }
                }

                if (activeBounties.Count == 0)
                {
                    Subject.Reply(source, "You have no active bounties to abandon.", "bountyboard_initial");

                    return;
                }

                foreach (var bountyName in activeBounties)
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_abandon_confirm",
                            OptionText = bountyName
                        });

                break;
            }
            case "bountyboard_abandon_confirm":
            {
                // The bounty's name is in Subject.Context due to OnNext passing it
                var chosenBounty = Subject.Context as string;

                if (string.IsNullOrEmpty(chosenBounty))
                {
                    Subject.Reply(source, "Invalid bounty selection to abandon.", "bountyboard_initial");

                    return;
                }

                // Provide "Yes" and "No" choices
                if (!Subject.HasOption("Yes"))
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_abandon_yes",
                            OptionText = "Yes"
                        });

                if (!Subject.HasOption("No"))

                    // Return them to the main board if they decline
                    Subject.Options.Add(
                        new DialogOption
                        {
                            DialogKey = "bountyboard_initial",
                            OptionText = "No"
                        });

                break;
            }

            case "bountyboard_abandon_yes":
            {
                var chosenBounty = Subject.Context as string;

                if (string.IsNullOrEmpty(chosenBounty))
                {
                    Subject.Reply(source, "Invalid bounty to abandon.", "bountyboard_initial");

                    return;
                }

                // Do the actual removal logic
                AbandonBounty(source, chosenBounty);

                Subject.Reply(source, $"You have abandoned the bounty: {chosenBounty}.", "bountyboard_initial");

                // Optionally jump back to the bountyboard_initial dialog
                break;
            }

            case "bountyboard_turnin1":
                ProcessBountyTurnIn(source, typeof(BountyBoardKill1));

                break;

            case "bountyboard_turnin2":
                ProcessBountyTurnIn(source, typeof(BountyBoardKill2));

                break;

            case "bountyboard_turnin3":
                ProcessBountyTurnIn(source, typeof(BountyBoardKill3));

                break;

            case "bountyboard_questinitial":
            {
                List<string> availableBounties;

                if (hasStage1 && hasStage2 && hasStage3)
                {
                    Subject.Reply(source, "You're currently on three bounties, you can't take anymore!", "bountyboard_initial");

                    return;
                }

                if (!source.Trackers.TimedEvents.HasActiveEvent("bountyboardreset", out _))
                {
                    if (source.Trackers.Flags.TryGetFlag(out BountyBoardOptions flags))
                        source.Trackers.Flags.RemoveFlag(flags);

                    var allBounties = GetAllBounties();
                    availableBounties = SelectRandomBounties(allBounties, 5);

                    source.Trackers.Counters.Remove("maxBountiesAccepted", out _);

                    foreach (var bounty in availableBounties)
                    {
                        (_, _, _, _, var bountyOption) = GetBountyFromAnyDictionary(bounty);
                        source.Trackers.Flags.AddFlag(bountyOption);
                    }

                    source.Trackers.TimedEvents.AddEvent("bountyboardreset", TimeSpan.FromHours(8), true);
                } else if (source.Trackers.Counters.TryGetValue("maxBountiesAccepted", out var count) && (count >= 3))
                {
                    Subject.Reply(source, "You can only accept three bounties per 8 hours.", "bountyboard_initial");

                    return;
                }

                availableBounties = RetrieveExistingBounties(source);

                if (availableBounties.Count == 0)
                {
                    Subject.Reply(source, "No bounties are available at this time.", "bountyboard_initial");

                    return;
                }

                foreach (var bounty in availableBounties)
                    if (!Subject.HasOption(bounty))
                        Subject.Options.Add(
                            new DialogOption
                            {
                                DialogKey = "bountyboard_acceptbounty",
                                OptionText = bounty
                            });

                break;
            }

            case "bountyboard_acceptbounty":
            {
                // Retrieve the selected bounty from the Context
                var selectedBounty = Subject.Context as string;

                if (string.IsNullOrEmpty(selectedBounty))
                {
                    Subject.Reply(source, "Invalid bounty selection.", "bountyboard_initial");

                    return;
                }

                // Merge all bounty dictionaries into a single one, preventing duplicates
                var bountyDictionary
                    = new Dictionary<string, (string MonsterKey, int KillRequirement, Enum KillEnum, BountyBoardFlags DifficultyFlag,
                        BountyBoardOptions BountyOption)>();

                foreach (var dict in new[]
                         {
                             GetBountyDictionary(typeof(BountyBoardKill1)),
                             GetBountyDictionary(typeof(BountyBoardKill2)),
                             GetBountyDictionary(typeof(BountyBoardKill3))
                         })
                {
                    foreach (var kvp in dict)
                        if (!bountyDictionary.ContainsKey(kvp.Key)) // Prevent duplicates
                            bountyDictionary[kvp.Key] = kvp.Value;
                }

                // Ensure the selected bounty exists
                if (!bountyDictionary.TryGetValue(selectedBounty, out var bountyData))
                {
                    Subject.Reply(source, "Invalid bounty selection.", "bountyboard_initial");

                    return;
                }

                // Extract data from the bounty
                (_, var killRequirement, var killEnum, var difficultyFlag, var bountyOption) = bountyData;

                // Determine which slot to use
                var hasBounty1 = source.Trackers.Enums.TryGetValue(out BountyBoardKill1 bounty1) && !bounty1.Equals(default);
                var hasBounty2 = source.Trackers.Enums.TryGetValue(out BountyBoardKill2 bounty2) && !bounty2.Equals(default);
                var hasBounty3 = source.Trackers.Enums.TryGetValue(out BountyBoardKill3 bounty3) && !bounty3.Equals(default);

                if (!hasBounty1)
                {
                    source.Trackers.Enums.Set(typeof(BountyBoardKill1), killEnum);
                    difficultyFlag = GetMatchingDifficultyFlag(difficultyFlag, 1);
                } else if (!hasBounty2)
                {
                    source.Trackers.Enums.Set(typeof(BountyBoardKill2), killEnum);
                    difficultyFlag = GetMatchingDifficultyFlag(difficultyFlag, 2);
                } else if (!hasBounty3)
                {
                    source.Trackers.Enums.Set(typeof(BountyBoardKill3), killEnum);
                    difficultyFlag = GetMatchingDifficultyFlag(difficultyFlag, 3);
                } else
                {
                    Subject.Reply(source, "You already have three active bounties. Complete one before accepting another.", "bountyboard_initial");

                    return;
                }

                // Apply the appropriate flag
                source.Trackers.Flags.AddFlag(difficultyFlag);
                source.Trackers.Counters.AddOrIncrement("maxBountiesAccepted");

                // Remove the bounty option flag so it does not reappear
                if (bountyOption != BountyBoardOptions.None)
                    source.Trackers.Flags.RemoveFlag(bountyOption);

                Subject.Reply(source, $"You've accepted the bounty: {selectedBounty}. Kill {killRequirement} to complete it!", "bountyboard_initial");

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is > 0 && (optionIndex <= Subject.Options.Count))
        {
            var chosenText = Subject.Options[optionIndex.Value - 1].OptionText;

            // Example of skipping the overwrite if we are in confirm step.
            if (Subject.Template.TemplateKey.Equals("bountyboard_abandon_confirm", StringComparison.OrdinalIgnoreCase))
            {
                if ((chosenText == "Yes") || (chosenText == "No"))
                {
                    // keep existing Subject.Context so we don't lose the bounty name
                    // do nothing here
                } else

                    // normal scenario
                    Subject.Context = chosenText;
            } else

                // normal scenario
                Subject.Context = chosenText;
        }

        base.OnNext(source, optionIndex);
    }

    private void ProcessBountyTurnIn(Aisling source, Type bountyEnumType)
    {
        if ((bountyEnumType == typeof(BountyBoardKill1)) && source.Trackers.Enums.TryGetValue(out BountyBoardKill1 bounty1))
            CompleteBounty(source, bounty1, bountyEnumType);
        else if ((bountyEnumType == typeof(BountyBoardKill2)) && source.Trackers.Enums.TryGetValue(out BountyBoardKill2 bounty2))
            CompleteBounty(source, bounty2, bountyEnumType);
        else if ((bountyEnumType == typeof(BountyBoardKill3)) && source.Trackers.Enums.TryGetValue(out BountyBoardKill3 bounty3))
            CompleteBounty(source, bounty3, bountyEnumType);
        else
            Subject.Reply(source, "You have no completed bounties to turn in.", "bountyboard_initial");
    }

    private List<string> RetrieveExistingBounties(Aisling source)
    {
        var availableBounties = new List<string>();

        if (!source.Trackers.Flags.TryGetFlag<BountyBoardOptions>(out var playerBountyOptions))
        {
            Subject.Reply(source, "No active bounties found.", "bountyboard_initial");

            return availableBounties;
        }

        foreach (var bountyDict in new[]
                 {
                     GetBountyDictionary(typeof(BountyBoardKill1))
                 })
        {
            foreach ((var bountyName, (_, _, _, _, var bountyOption)) in bountyDict)
                if (playerBountyOptions.HasFlag(bountyOption))
                    availableBounties.Add(bountyName);
        }

        return availableBounties;
    }

    private List<string> SelectRandomBounties(List<string> allBounties, int count)
    {
        var selectedBounties = new List<string>();
        var selectedMonsters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Filter out epic quests BEFORE shuffling
        var filteredBounties = allBounties
                               .Where(bounty => !bounty.Contains("({=pEpic", StringComparison.OrdinalIgnoreCase)) // Exclude Epic bounties
                               .ToList();

        // Shuffle filtered list before selection
        var shuffledBounties = filteredBounties.OrderBy(_ => Random.Next())
                                               .ToList();

        foreach (var bounty in shuffledBounties)
        {
            (var monsterKey, _, _, _, _) = GetBountyFromAnyDictionary(bounty);

            // Skip if this monster was already selected at another difficulty
            if (selectedMonsters.Contains(monsterKey))
                continue;

            // Add to selected lists
            selectedBounties.Add(bounty);
            selectedMonsters.Add(monsterKey);

            // Stop when reaching the required count
            if (selectedBounties.Count == count)
                break;
        }

        return selectedBounties;
    }
}