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

namespace Chaos.Scripting.DialogScripts.Quests.MilethQuest;

public class CrudeLeatherQuestScript : DialogScriptBase
{
    private readonly ILogger<CrudeLeatherQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public CrudeLeatherQuestScript(Dialog subject, ILogger<CrudeLeatherQuestScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out CrudeLeather stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "eiganjo_initial":
            {
                
                var option = new DialogOption
                {
                    DialogKey = "crudeleather_initial",
                    OptionText = "Crude Leather"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "crudeleather_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("crudeleathercd", out var cdtime))
                {
                    Subject.Reply(source, $"Thank you again Aisling for those wolf furs. I am working on a new Crude Leather Breast plate as we speak. I could probably use another ten tomorrow. Come back and see me at (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                if (hasStage && stage == CrudeLeather.StartedQuest)
                {
                    Subject.Reply(source, "Skip", "crudeleather_return");
                }
                break;
            }

            case "crudeleather_start2":
            {
                source.Trackers.Enums.Set(CrudeLeather.StartedQuest);
                source.SendOrangeBarMessage("Retrieve 10 Wolf's Fur for Eiganjo.");
                break;
            }

            case "crudeleather_turnin":
            {
                var hasRequiredWolfFurs = source.Inventory.HasCount("Wolf's Fur", 10);
                
                if (hasStage && (stage == CrudeLeather.StartedQuest))
                {
                    switch (hasRequiredWolfFurs)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Wolf's Fur", 10, out _);
                            source.Trackers.Enums.Set(CrudeLeather.None);
                            source.Trackers.TimedEvents.AddEvent("crudeleathercd", TimeSpan.FromHours(24), true);

                            Logger.WithTopics(
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
                                      15000);

                            ExperienceDistributionScript.GiveExp(source, 15000);
                            source.TryGiveGold(5000);
                            source.TryGiveGamePoints(5);

                            if (IntegerRandomizer.RollChance(8))
                            {
                                source.Legend.AddOrAccumulate(
                                    new LegendMark(
                                        "Loved by Mileth Mundanes",
                                        "milethLoved",
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
                            var wolfFurCount = source.Inventory.CountOf("Wolf's Fur");

                            if (wolfFurCount < 1)
                            {
                                Subject.Reply(source, "You don't even have one! Please go get those wolf's fur.");
                                return;
                            }
                        
                            Subject.Reply(source,
                                $"Can you not count? What am I going to do with {wolfFurCount} wolf furs? I need at least 10 to make one crude leather breast plate for my students.");

                            break;
                        }
                    }
                }

                break;
            }
        }
    }
}