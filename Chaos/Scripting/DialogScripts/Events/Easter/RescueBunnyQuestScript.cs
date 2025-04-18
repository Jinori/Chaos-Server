using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Easter;

public class RescueBunnyQuestScript(Dialog subject, ILogger<RescueBunnyQuestScript> logger, IItemFactory itemFactory)
    : DialogScriptBase(subject)
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out BunnyRescue stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyFivePercent = MathEx.GetPercentOf<int>(tnl, 25);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "silasbunnium_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "bunnyrescue_initial",
                    OptionText = "Bunny Rescue"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "bunnyrescue_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("bunnyrescuedcd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"I have all the bunnies for now, but if one escapes, I'll let you know! Check back in {cdtime.Remaining.ToReadableString()}.");

                    return;
                }

                if (hasStage && (stage == BunnyRescue.StartedQuest))
                    Subject.Reply(source, "Skip", "bunnyrescue_return1");

                break;
            }

            case "bunnyrescue_start1":
            {
                source.Trackers.Enums.Set(BunnyRescue.StartedQuest);
                source.SendOrangeBarMessage("Find ten of the bunnies that ran away.");

                break;
            }

            case "bunnyrescue_turnin":
            {
                if (hasStage && (stage == BunnyRescue.StartedQuest))
                {
                    // Get the current count of Baby BunnyRescues turned in
                    if (!source.Trackers.Counters.TryGetValue("BunnyRescueCount", out var currentCount))
                        currentCount = 0; // Initialize to 0 if not set

                    var babyBunnyRescueCount = source.Inventory.CountOf("Rescued Bunny");
                    var needed = 10 - currentCount;

                    if ((currentCount > 0) || (babyBunnyRescueCount > 0))
                    {
                        var toTurnIn = Math.Min(babyBunnyRescueCount, needed);
                        source.Inventory.RemoveQuantity("Rescued Bunny", toTurnIn, out _);
                        currentCount += toTurnIn;

                        // Update the counter in Trackers
                        source.Trackers.Counters.Set("BunnyRescueCount", currentCount);

                        if (currentCount >= 10)
                        {
                            // Reset the counter and reward the player
                            source.Trackers.Enums.Set(BunnyRescue.None);
                            source.Trackers.Counters.Remove("BunnyRescueCount", out _); // Remove counter after completion
                            source.Trackers.TimedEvents.AddEvent("bunnyrescuedcd", TimeSpan.FromHours(6), true);

                            var expRewarded = source.UserStatSheet.Level == 99 ? 10000000 : twentyFivePercent;
                            ExperienceDistributionScript.GiveExp(source, expRewarded);
                            source.TryGiveGamePoints(5);
                            var magicianhat = ItemFactory.Create("magicianhat");
                            source.GiveItemOrSendToBank(magicianhat);

                            source.SendOrangeBarMessage("You have found all the bunnies! Thank you for your help.");
                            Subject.Reply(source, "There's all my bunnies! The show must go on! Thank you Aisling.");

                            logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@ExpAmount} exp from returning rescued bunnies",
                                      source.Name,
                                      expRewarded);
                        } else
                            Subject.Reply(
                                source,
                                $"You have returned {currentCount} bunnies. Keep searching for the rest of my poor bunnies!");
                    } else
                        Subject.Reply(source, "You haven't found any bunnies yet. Please keep searching the fields!");
                }

                break;
            }
        }
    }
}