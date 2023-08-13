using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Quests;

public class HolyResearchQuestScript : DialogScriptBase
{
    private readonly ILogger<HolyResearchQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HolyResearchQuestScript(Dialog subject, ILogger<HolyResearchQuestScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    private void GiveRewards(Aisling source, string itemToRemove)
    {
        Logger.WithTopics(
                  Topics.Entities.Aisling,
                  Topics.Entities.Experience,
                  Topics.Entities.Dialog,
                  Topics.Entities.Quest)
              .WithProperty(source)
              .WithProperty(Subject)
              .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, 2000);

        source.Inventory.RemoveQuantity(itemToRemove, 1);
        ExperienceDistributionScript.GiveExp(source, 2000);
        source.Trackers.Enums.Set(HolyResearchStage.None);
        source.TryGiveGamePoints(5);
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You receive five gamepoints and 2000 exp!");
        Subject.Close(source);
        source.Trackers.TimedEvents.AddEvent("HolyResearchCd", TimeSpan.FromHours(3), true);
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out HolyResearchStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "berteli_initial":
                if (source.UserStatSheet.Level > 50)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "HolyResearch_initial",
                    OptionText = "Holy Research"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;

            case "holyresearch_initial":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("HolyResearchCd", out var timedEvent))
                    {
                        Subject.Reply(
                            source,
                            $"I don't need any more right now, please come back later. (({timedEvent.Remaining.ToReadableString()
                            }))");

                        return;
                    }

                    return;
                }

                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    Subject.Reply(source, "skip", "holyresearch_rh");

                    return;
                }

                if (stage == HolyResearchStage.StartedRawWax)
                {
                    Subject.Reply(source, "skip", "holyresearch_rw");

                    return;
                }

                if (stage == HolyResearchStage.StartedRoyalWax)
                    Subject.Reply(source, "skip", "holyresearch_royw");

                break;

            case "holyresearch_yes":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    var randomHolyResearchStage = new[]
                    {
                        HolyResearchStage.StartedRawHoney, HolyResearchStage.StartedRawWax, HolyResearchStage.StartedRoyalWax
                    }.PickRandom();

                    source.Trackers.Enums.Set(randomHolyResearchStage);

                    switch (randomHolyResearchStage)
                    {
                        case HolyResearchStage.StartedRawHoney:
                        {
                            Subject.Reply(source, "Thank you so much Aisling, bring me one raw honey.");
                        }

                            break;

                        case HolyResearchStage.StartedRawWax:
                        {
                            Subject.Reply(source, "Thank you so much Aisling, bring me one raw wax.");
                        }

                            break;

                        case HolyResearchStage.StartedRoyalWax:
                        {
                            Subject.Reply(source, "Thank you so much Aisling, bring me one royal wax.");
                        }

                            break;
                    }
                }

                break;

            case "holyresearch_startedrawhoney":
                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    if (!source.Inventory.HasCount("raw honey", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    GiveRewards(source, "raw honey");
                }

                break;

            case "holyresearch_startedrawwax":
                if (stage == HolyResearchStage.StartedRawWax)
                {
                    if (!source.Inventory.HasCount("raw wax", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    GiveRewards(source, "raw wax");
                }

                break;

            case "holyresearch_startedroyalwax":
                if (stage == HolyResearchStage.StartedRoyalWax)
                {
                    if (!source.Inventory.HasCount("royal wax", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    GiveRewards(source, "royal wax");
                }

                break;
        }
    }
}