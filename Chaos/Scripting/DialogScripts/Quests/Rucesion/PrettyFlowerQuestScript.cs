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

namespace Chaos.Scripting.DialogScripts.Quests.Rucesion;

public class PrettyFlowerQuestScript(Dialog subject, ILogger<PrettyFlowerQuestScript> logger)
    : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out PrettyFlower stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "maria_initial":
            {
                if (source.UserStatSheet.Level < 11)
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "prettyflower_initial",
                    OptionText = "Pretty Flower"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "prettyflower_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("prettyflowercd", out var cdtime))
                {
                    Subject.Reply(source, $"I'm grateful for that flower, it makes me smile in the morning seeing it there. I'm sure it'll expire soon though, I could use another. (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                if (hasStage && (stage == PrettyFlower.StartedQuest))
                {
                    Subject.Reply(source, "Skip", "prettyflower_return");
                }
                break;
            }

            case "prettyflower_start2":
            {
                source.Trackers.Enums.Set(PrettyFlower.StartedQuest);
                source.SendOrangeBarMessage("Retrieve a Kobold Tail for Maria");
                break;
            }

            case "prettyflower_turnin":
            {
                var hasRequiredFlower = source.Inventory.HasCount("Kobold Tail", 1);
                
                if (hasStage && (stage == PrettyFlower.StartedQuest))
                    switch (hasRequiredFlower)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Kobold Tail", 1, out _);
                            source.Trackers.Enums.Set(PrettyFlower.None);
                            source.Trackers.TimedEvents.AddEvent("prettyflowercd", TimeSpan.FromHours(24), true);

                            logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@ExpAmount} exp from a quest",
                                      source.Name,
                                      20000);

                            ExperienceDistributionScript.GiveExp(source, 20000);
                            source.TryGiveGamePoints(5);

                            if (IntegerRandomizer.RollChance(8))
                            {
                                source.Legend.AddOrAccumulate(
                                    new LegendMark(
                                        "Loved by Rucesion Mundanes",
                                        "rucesionLoved",
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
                            Subject.Reply(source,
                                $"Awe, you don't have the Kobold Tail, it's such a pretty flower. When you find one, please bring it back to me.");

                            break;
                    }

                break;
            }
        }
    }
}