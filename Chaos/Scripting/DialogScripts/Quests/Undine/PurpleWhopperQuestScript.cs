using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
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
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Undine;

public class PurpleWhopperQuestScript(Dialog subject, ILogger<PurpleWhopperQuestScript> logger)
    : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out PurpleWhopper stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ayumi_initial":
            {
                
                var option = new DialogOption
                {
                    DialogKey = "purplewhopper_initial",
                    OptionText = "Purple Whopper Scales"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "purplewhopper_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("purplewhoppercd", out var cdtime))
                {
                    Subject.Reply(source, $"*Ayumi seems a bit out of it* o you back, that some gud fish. I like more soon. See me in (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                if (hasStage && stage == PurpleWhopper.StartedQuest)
                {
                    Subject.Reply(source, "Skip", "purplewhopper_return");
                }
                break;
            }

            case "purplewhopper_start2":
            {
                source.Trackers.Enums.Set(PurpleWhopper.StartedQuest);
                source.SendOrangeBarMessage("Bring Ayumi a Purple Whopper");
                break;
            }

            case "purplewhopper_turnin":
            {
                var hasRequiredPurpleWhopper = source.Inventory.HasCount("Purple Whopper", 1);
                
                if (hasStage && stage == PurpleWhopper.StartedQuest)
                {
                    switch (hasRequiredPurpleWhopper)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Purple Whopper", 1, out _);
                            source.Trackers.Enums.Set(PurpleWhopper.None);
                            source.Trackers.TimedEvents.AddEvent("purplewhoppercd", TimeSpan.FromHours(22), true);

                            logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from a quest",
                                      source.Name,
                                      5000,
                                      25000);

                            ExperienceDistributionScript.GiveExp(source, 25000);
                            source.TryGiveGold(10000);
                            source.TryGiveGamePoints(5);

                            if (IntegerRandomizer.RollChance(8))
                            {
                                source.Legend.AddOrAccumulate(
                                    new LegendMark(
                                        "Loved by Undine Mundanes",
                                        "undineLoved",
                                        MarkIcon.Heart,
                                        MarkColor.Blue,
                                        1,
                                        GameTime.Now));

                                source.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                                    "You received a unique legend mark!");
                            }

                            break;
                        }
                        case false:
                        {
                                Subject.Reply(source, "There no fish there. Where fish?");
                                return;
                        }
                    }
                }

                break;
            }
        }
    }
}