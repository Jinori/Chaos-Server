using System.Text;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.BountyBoard;

public class BountyBoardDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BountyBoardDialogScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public BountyBoardDialogScript(Dialog subject, IItemFactory itemFactory, ILogger<BountyBoardDialogScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }
    
    private static readonly List<KeyValuePair<string, decimal>> EpicRewards =
    [
        new("nyxtwilightband", 10),
        new("nyxumbralshield", 6),
        new("nyxwhisper", 8),
        new("nyxembrace", 8),
        new("", 5),
        new("", 8),
        new("", 6),
        new("", 2)
    ];

    private IEnumerable<BountyDetails> GetCurrentBounties(Aisling source)
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

    private List<BountyDetails> GrabAislingAvailableBounties(Aisling source)
        => BountyBoardQuests.PossibleQuestDetails
                            .Where(bounty => source.Trackers.Flags.HasFlag(bounty.AvailableQuestFlag.GetType(), bounty.AvailableQuestFlag))
                            .ToList();

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey)
        {
            case "bountyboard_initial":
            {
                if (!source.UserStatSheet.Master)
                {
                    Subject.Reply(source, "You don't understand anything on the board. (Requires Master)");

                    return;
                }

                if (source.Trackers.Counters.CounterGreaterThanOrEqualTo("epicBounty", 10))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "bountyboard_epicbountyinitial",
                        OptionText = "Epic Bounty"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                var epicMarkCount = source.Legend.GetCount("epicbounty");
                
                if ((epicMarkCount >= 100) && !source.Titles.ContainsI("Bounty Master"))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "bountyboard_epicbountyreward",
                        OptionText = "Bounty Master"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "bountyboard_questinitial":
            {
                var flagCount = GetCurrentBounties(source);

                var flags = flagCount.ToList();

                if (flags.Count >= 3)
                {
                    Subject.Reply(source, "You may only have three active bounties at a time.", "bountyboard_initial");

                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent("bountyboard_reset", out _))
                {
                    if (source.Trackers.Counters.TryGetValue("bountycount", out var count) && (count >= 3))
                    {
                        Subject.Reply(source, "You may not take anymore bounties today.", "bountyboard_initial");

                        return;
                    }

                    var currentAvailableBounties = GrabAislingAvailableBounties(source);
                    Subject.Context = currentAvailableBounties;
                    var options = currentAvailableBounties.Select(bounty => (bounty.QuestText, "bountyboard_acceptbounty"));
                    Subject.AddOptions(options);
                } else
                {
                    var possibleBounties = SelectRandomBounties(source);

                    Subject.Context = possibleBounties;

                    var options2 = possibleBounties.Select(bounty => (bounty.QuestText, "bountyboard_acceptbounty"));
                    Subject.AddOptions(options2);

                    var previousBounties = source.Trackers.Flags.TryGetFlag(out AvailableQuestFlags1 flags1);

                    if (previousBounties)
                        source.Trackers.Flags.RemoveFlag(flags1);

                    foreach (var bounty in possibleBounties)
                        source.Trackers.Flags.AddFlag(bounty.AvailableQuestFlag.GetType(), bounty.AvailableQuestFlag);

                    source.Trackers.TimedEvents.AddEvent("bountyboard_reset", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("bountycount", out _);
                }

                break;
            }
            case "bountyboard_epicbountyinitial":
            {
                var flagCount = GetCurrentBounties(source);

                var flags = flagCount.ToList();

                if (flags.Count >= 3)
                    Subject.Reply(
                        source,
                        "You can only have three active bounties at a time. You must abandon or complete a normal bounty to free up a slot in order to accept an Epic Bounty.",
                        "bountyboard_initial");

                if (source.Trackers.Flags.HasFlag(BountyQuestFlags1.EpicAncientDraco)
                    || source.Trackers.Flags.HasFlag(BountyQuestFlags1.EpicKelberoth)
                    || source.Trackers.Flags.HasFlag(BountyQuestFlags1.EpicHydra)
                    || source.Trackers.Flags.HasFlag(BountyQuestFlags1.EpicGreenMantis))
                    Subject.Reply(
                        source,
                        "You may only have one Epic Bounty active at a time. Please abandon or complete your current Epic Bounty to accept another.");

                break;
            }

            case "bountyboard_acceptbounty":
            {
                if (Subject.Context is not BountyDetails bountyDetails)
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                //after they select the quest, Subject.Context contains bountyDetails
                Subject.InjectTextParameters(bountyDetails.QuestText);

                break;
            }
            case "bountyboard_viewactive":
            {
                var currentBounties = GetCurrentBounties(source)
                    .ToList();

                if (currentBounties.Count < 1)
                {
                    Subject.Reply(source, "You are not actively pursuing any bounties.", "bountyboard_initial");

                    return;
                }

                var builder = new StringBuilder();

                foreach (var bounty in currentBounties)
                    builder.AppendLine(bounty.QuestText);

                Subject.InjectTextParameters(
                    builder.ToString()
                           .FixLineEndings());

                break;
            }
            case "bountyboard_abandon":
            {
                var currentBounties = GetCurrentBounties(source)
                    .ToList();

                if (currentBounties.Count < 1)
                {
                    Subject.Reply(source, "You are not actively pursuing any bounties.", "bountyboard_initial");

                    return;
                }

                var options = currentBounties.Select(bounty => (bounty.QuestText, "bountyboard_abandon_confirm"));
                Subject.AddOptions(options);
                Subject.Context = currentBounties;

                break;
            }
            case "bountyboard_abandon_confirm":
            {
                if (Subject.Context is not BountyDetails bountyDetails)
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                Subject.InjectTextParameters(bountyDetails.QuestText);

                break;
            }

            case "bountyboard_turnin":
            {
                var currentBounties = GetCurrentBounties(source)
                    .ToList();

                if (currentBounties.Count < 1)
                {
                    Subject.Reply(source, "You are not actively pursuing any bounties.", "bountyboard_initial");

                    return;
                }

                var options = currentBounties.Select(bounty => (bounty.QuestText, "bountyboard_turninbounty"));
                Subject.AddOptions(options);
                Subject.Context = currentBounties;

                break;
            }
            case "bountyboard_turninbounty":
            {
                if (Subject.Context is not BountyDetails)
                    Subject.ReplyToUnknownInput(source);

                break;
            }

            case "bountyboard_epicbountyreward":
            {
                source.Titles.Add("Bounty Master");
                source.SendOrangeBarMessage("Visit Goddess Skandara in the God's Realm immediately.");
                
                Logger.WithTopics(
                          [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation(
                          "{@AislingName} has received Bounty Master title.",
                          source.Name);
                
                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey)
        {
            case "bountyboard_questinitial":
            {
                switch (optionIndex!.Value)
                {
                    case 1:
                    {
                        if (Subject.Context is not List<BountyDetails> bountyDetails)
                        {
                            Subject.ReplyToUnknownInput(source);

                            return;
                        }

                        var selectedBounty = bountyDetails[0];

                        source.Trackers.Flags.AddFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                        source.Trackers.Counters.AddOrIncrement("bountycount", 1);
                        source.SendOrangeBarMessage($"You accepted {selectedBounty.QuestText}.");
                        source.Trackers.Flags.RemoveFlag(selectedBounty.AvailableQuestFlag.GetType(), selectedBounty.AvailableQuestFlag);
                        Subject.Context = selectedBounty;
                        
                        Logger.WithTopics(
                                  [Topics.Entities.Aisling,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest])
                              .WithProperty(source)
                              .WithProperty(Subject)
                              .LogInformation(
                                  "{@AislingName} has accepted {@QuestText} bounty.",
                                  source.Name,
                                  selectedBounty.QuestText);

                        break;
                    }
                    case 2:
                    {
                        if (Subject.Context is not List<BountyDetails> bountyDetails)
                        {
                            Subject.ReplyToUnknownInput(source);

                            return;
                        }

                        var selectedBounty = bountyDetails[1];

                        source.Trackers.Flags.AddFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                        source.Trackers.Counters.AddOrIncrement("bountycount", 1);
                        source.SendOrangeBarMessage($"You accepted {selectedBounty.QuestText}.");
                        source.Trackers.Flags.RemoveFlag(selectedBounty.AvailableQuestFlag.GetType(), selectedBounty.AvailableQuestFlag);
                        Subject.Context = selectedBounty;
                        
                        Logger.WithTopics(
                                  [Topics.Entities.Aisling,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest])
                              .WithProperty(source)
                              .WithProperty(Subject)
                              .LogInformation(
                                  "{@AislingName} has accepted {@QuestText} bounty.",
                                  source.Name,
                                  selectedBounty.QuestText);

                        break;
                    }
                    case 3:
                    {
                        if (Subject.Context is not List<BountyDetails> bountyDetails)
                        {
                            Subject.ReplyToUnknownInput(source);

                            return;
                        }

                        var selectedBounty = bountyDetails[2];

                        source.Trackers.Flags.AddFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                        source.Trackers.Counters.AddOrIncrement("bountycount", 1);
                        source.SendOrangeBarMessage($"You accepted {selectedBounty.QuestText}.");
                        source.Trackers.Flags.RemoveFlag(selectedBounty.AvailableQuestFlag.GetType(), selectedBounty.AvailableQuestFlag);
                        Subject.Context = selectedBounty;
                        
                        Logger.WithTopics(
                                  [Topics.Entities.Aisling,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest])
                              .WithProperty(source)
                              .WithProperty(Subject)
                              .LogInformation(
                                  "{@AislingName} has accepted {@QuestText} bounty.",
                                  source.Name,
                                  selectedBounty.QuestText);

                        break;
                    }
                    case 4:
                    {
                        if (Subject.Context is not List<BountyDetails> bountyDetails)
                        {
                            Subject.ReplyToUnknownInput(source);

                            return;
                        }

                        var selectedBounty = bountyDetails[3];

                        source.Trackers.Flags.AddFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                        source.Trackers.Counters.AddOrIncrement("bountycount", 1);
                        source.SendOrangeBarMessage($"You accepted {selectedBounty.QuestText}.");
                        source.Trackers.Flags.RemoveFlag(selectedBounty.AvailableQuestFlag.GetType(), selectedBounty.AvailableQuestFlag);
                        Subject.Context = selectedBounty;
                        
                        Logger.WithTopics(
                                  [Topics.Entities.Aisling,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest])
                              .WithProperty(source)
                              .WithProperty(Subject)
                              .LogInformation(
                                  "{@AislingName} has accepted {@QuestText} bounty.",
                                  source.Name,
                                  selectedBounty.QuestText);

                        break;
                    }
                    case 5:
                    {
                        if (Subject.Context is not List<BountyDetails> bountyDetails)
                        {
                            Subject.ReplyToUnknownInput(source);

                            return;
                        }

                        var selectedBounty = bountyDetails[4];

                        source.Trackers.Flags.AddFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                        source.Trackers.Counters.AddOrIncrement("bountycount", 1);
                        source.SendOrangeBarMessage($"You accepted {selectedBounty.QuestText}.");
                        source.Trackers.Flags.RemoveFlag(selectedBounty.AvailableQuestFlag.GetType(), selectedBounty.AvailableQuestFlag);
                        Subject.Context = selectedBounty;
                        
                        Logger.WithTopics(
                                  [Topics.Entities.Aisling,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest])
                              .WithProperty(source)
                              .WithProperty(Subject)
                              .LogInformation(
                                  "{@AislingName} has accepted {@QuestText} bounty.",
                                  source.Name,
                                  selectedBounty.QuestText);
                    }

                        break;
                }

                break;
            }

            case "bountyboard_abandon":
            {
                if (Subject.Context is not List<BountyDetails> bountyDetails)
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var selectedBounty = bountyDetails[optionIndex!.Value - 1];
                Subject.Context = selectedBounty;

                break;
            }
            case "bountyboard_abandon_confirm":
            {
                if (Subject.Context is not BountyDetails bountyDetails)
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                //if confirmed
                if (optionIndex!.Value == 1)
                {
                    source.Trackers.Flags.RemoveFlag(bountyDetails.BountyQuestFlag.GetType(), bountyDetails.BountyQuestFlag);
                    source.Trackers.Counters.Remove(bountyDetails.MonsterTemplateKey, out _);
                    source.SendOrangeBarMessage($"You abandoned {bountyDetails.QuestText}.");

                    Subject.Context = bountyDetails.QuestText;
                    
                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                                  Topics.Entities.Experience,
                                  Topics.Entities.Dialog,
                                  Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation(
                              "{@AislingName} has abandoned {@QuestText} bounty.",
                              source.Name,
                              bountyDetails.QuestText);

                    Subject.Reply(source, $"You've abandoned {bountyDetails.QuestText}", "bountyboard_initial");
                }

                break;
            }

            case "bountyboard_epicbountyinitial":
            {
                if (optionIndex!.Value == 1)
                {
                    var epicBountyList = SelectRandomEpicBounty(source)
                        .ToList();

                    var selectedBounty = epicBountyList[0];

                    source.Trackers.Flags.AddFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                    source.Trackers.Counters.TryDecrement("epicbounty", 10, out _);
                    source.SendOrangeBarMessage($"You accepted {selectedBounty.QuestText}.");

                    Subject.Context = selectedBounty.QuestText;
                    
                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                                  Topics.Entities.Dialog,
                                  Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation(
                              "{@AislingName} has accepted {@QuestText} bounty.",
                              source.Name,
                              selectedBounty.QuestText);
                    

                    Subject.Reply(source, $"You've accepted {selectedBounty.QuestText}!", "bountyboard_initial");
                }

                break;
            }

            case "bountyboard_turnin":
            {
                if (Subject.Context is not List<BountyDetails> bountyDetails)
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var selectedBounty = bountyDetails[optionIndex!.Value - 1];

                Subject.Context = selectedBounty;

                if (source.Trackers.Counters.CounterGreaterThanOrEqualTo(selectedBounty.MonsterTemplateKey, selectedBounty.KillRequirement))
                {
                    var gamepoints = 0;
                    var exp = 0;
                    var bountyPoints = 0;

                    source.Trackers.Flags.RemoveFlag(selectedBounty.BountyQuestFlag.GetType(), selectedBounty.BountyQuestFlag);
                    source.Trackers.Counters.Remove(selectedBounty.MonsterTemplateKey, out _);

                    if (selectedBounty.KillRequirement == 100)
                    {
                        exp = 50000000;
                        gamepoints = 20;
                        bountyPoints = 1;
                    } else if (selectedBounty.KillRequirement == 250)
                    {
                        exp = 150000000;
                        gamepoints = 50;
                        bountyPoints = 2;
                    } else if (selectedBounty.KillRequirement == 400)
                    {
                        exp = 250000000;
                        gamepoints = 75;
                        bountyPoints = 3;
                    } else if (selectedBounty.KillRequirement == 10)
                    {
                        exp = 300000000;
                        gamepoints = 100;

                        var item = EpicRewards.PickRandomWeighted();
                        var itemReward = ItemFactory.Create(item);
                        
                        if (itemReward.DisplayName.ContainsI("Nyx"))
                            source.Trackers.Counters.AddOrIncrement($"NyxItem{itemReward.UniqueId}");
                        
                        source.GiveItemOrSendToBank(itemReward);
                        
                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Completed an Epic Bounty",
                                "epicbounty",
                                MarkIcon.Victory,
                                MarkColor.DarkPurple,
                                1,
                                GameTime.Now));
                    }

                    ExperienceDistributionScript.GiveExp(source, exp);
                    source.TryGiveGamePoints(gamepoints);
                    source.Trackers.Counters.AddOrIncrement("epicBounty", bountyPoints);
                    
                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                                  Topics.Entities.Experience,
                                  Topics.Entities.Dialog,
                                  Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation(
                              "{@AislingName} has received {@ExpAmount} exp and {@GamePoints} from {@QuestText} bounty.",
                              source.Name,
                              exp,
                              gamepoints,
                              selectedBounty.QuestText);

                    Subject.Reply(
                        source,
                        $"Congratulations! You've completed {selectedBounty.QuestText}! You received {exp} experience and {gamepoints} gamepoints!",
                        "bountyboard_initial");
                } else
                {
                    var killCount = source.Trackers.Counters.TryGetValue(selectedBounty.MonsterTemplateKey, out var k) ? k : 0;

                    Subject.Reply(
                        source,
                        $"You haven't completed this bounty. You're current progress is {killCount}/{selectedBounty.KillRequirement}!",
                        "bountyboard_initial");
                }

                break;
            }
        }
    }

    private List<BountyDetails> SelectRandomBounties(Aisling source)
    {
        var activeBounties = GetCurrentBounties(source)
                             .Select(b => b.BountyQuestFlag)
                             .ToHashSet();

        var activeMonsterKeys = BountyBoardQuests.PossibleQuestDetails
                                                 .Where(q => activeBounties.Contains(q.BountyQuestFlag))
                                                 .Select(q => q.MonsterTemplateKey)
                                                 .ToHashSet();

        return BountyBoardQuests.PossibleQuestDetails
                                .Where(bounty => !bounty.QuestText.ContainsI("({=pEpic"))
                                .Where(bounty => !source.Trackers.Flags.HasFlag(bounty.BountyQuestFlag.GetType(), bounty.BountyQuestFlag))
                                .Where(bounty => !activeMonsterKeys.Contains(bounty.MonsterTemplateKey))
                                .Shuffle()
                                .DistinctBy(bounty => bounty.MonsterTemplateKey)
                                .Take(5)
                                .ToList();
    }

    private List<BountyDetails> SelectRandomEpicBounty(Aisling source)
    {
        var activeBounties = GetCurrentBounties(source)
                             .Select(b => b.BountyQuestFlag)
                             .ToHashSet();

        var activeMonsterKeys = BountyBoardQuests.PossibleQuestDetails
                                                 .Where(q => activeBounties.Contains(q.BountyQuestFlag))
                                                 .Select(q => q.MonsterTemplateKey)
                                                 .ToHashSet();

        return BountyBoardQuests.PossibleQuestDetails
                                .Where(bounty => bounty.QuestText.ContainsI("({=pEpic"))
                                .Where(bounty => !source.Trackers.Flags.HasFlag(bounty.BountyQuestFlag.GetType(), bounty.BountyQuestFlag))
                                .Where(bounty => !activeMonsterKeys.Contains(bounty.MonsterTemplateKey))
                                .DistinctBy(bounty => bounty.MonsterTemplateKey)
                                .Shuffle()
                                .Take(1)
                                .ToList();
    }
}