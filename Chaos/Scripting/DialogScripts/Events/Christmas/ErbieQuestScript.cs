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

namespace Chaos.Scripting.DialogScripts.Events.Christmas;

public class ErbieQuestScript(Dialog subject, ILogger<Erbie> logger, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out Erbie stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyFivePercent = MathEx.GetPercentOf<int>(tnl, 25);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "mothererbie_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "erbie_initial",
                    OptionText = "Baby Erbies"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "erbie_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("babyerbiecd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"Oh I know, All my babies are home down, resting safely. Check back in {cdtime.Remaining.ToReadableString()}.");

                    return;
                }

                if (hasStage && (stage == Erbie.StartedQuest))
                    Subject.Reply(source, "Skip", "erbie_return1");

                break;
            }

            case "erbie_start1":
            {
                source.Trackers.Enums.Set(Erbie.StartedQuest);
                source.SendOrangeBarMessage("Find ten of the baby erbies that ran away.");

                break;
            }

            case "erbie_turnin":
            {
                if (hasStage && (stage == Erbie.StartedQuest))
                {
                    // Get the current count of Baby Erbies turned in
                    if (!source.Trackers.Counters.TryGetValue("BabyErbieCount", out var currentCount))
                        currentCount = 0; // Initialize to 0 if not set

                    var babyErbieCount = source.Inventory.CountOf("Baby Erbie");
                    var needed = 10 - currentCount;

                    if (babyErbieCount > 0)
                    {
                        var toTurnIn = Math.Min(babyErbieCount, needed);
                        source.Inventory.RemoveQuantity("Baby Erbie", toTurnIn, out _);
                        currentCount += toTurnIn;

                        // Update the counter in Trackers
                        source.Trackers.Counters.Set("BabyErbieCount", currentCount);

                        if (currentCount >= 10)
                        {
                            // Reset the counter and reward the player
                            source.Trackers.Enums.Set(Erbie.None);
                            source.Trackers.Counters.Remove("BabyErbieCount", out _); // Remove counter after completion
                            source.Trackers.TimedEvents.AddEvent("babyerbiecd", TimeSpan.FromHours(6), true);

                            var expRewarded = source.UserStatSheet.Level == 99 ? 10000000 : twentyFivePercent;
                            ExperienceDistributionScript.GiveExp(source, expRewarded);
                            source.TryGiveGamePoints(5);
                            var erbiegiftbox = ItemFactory.Create("erbiegiftbox");
                            source.GiveItemOrSendToBank(erbiegiftbox);

                            source.SendOrangeBarMessage("You have found all the Baby Erbies! Thank you for your help.");

                            logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@ExpAmount} exp from returning Baby Erbies.",
                                      source.Name,
                                      expRewarded);
                        } else
                            Subject.Reply(source, $"You have returned {toTurnIn} Baby Erbies. Keep searching for the rest of my babies!");
                    } else
                        Subject.Reply(source, "You haven't found any Baby Erbies yet. Please keep searching!");
                }

                break;
            }
        }
    }
}