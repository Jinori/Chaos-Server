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
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Rucesion;

public class FishOilQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<FishOilQuestScript> logger)
    : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out FishOil stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "antonio_initial":
            {
                
                var option = new DialogOption
                {
                    DialogKey = "fishoil_initial",
                    OptionText = "*Antonio's Vault squeeks*"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }
            
            case "kamel_initial":
            {
                if (hasStage && (stage == FishOil.StartedQuest))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "fishoil_initial2",
                        OptionText = "Extract Fish Oil"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
   
                }
                break;
            }

            case "fishoil_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("fishoilcd", out var cdtime))
                {
                    Subject.Reply(source, $"That door is good as new. I couldn't of done it without your help. Thanks again. I'm sure I'll need more soon. (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                if (hasStage && (stage == FishOil.StartedQuest))
                    Subject.Reply(source, "Skip", "fishoil_return");

                break;
            }

            case "fishoil_extract2":
            {
                var hasRequiredFish = source.Inventory.HasCount("Lion Fish", 5);
                
                switch (hasRequiredFish)
                {
                    case true:
                        source.Inventory.RemoveQuantity("Lion Fish", 5);
                        var oil = itemFactory.Create("fishoil");
                        source.GiveItemOrSendToBank(oil);
                        Subject.Reply(source, "Thanks for the fish, here's some oil I had lying around from some previous fish. I'll make more later.", "Close");
                        source.SendOrangeBarMessage("You received oil.");
                        return;
                    
                    case false:
                        Subject.Reply(source, "This won't be enough fish to make enough oil for a jar. Come back when you have some more fish.");
                        break;
                }

                break;
            }

            case "fishoil_start2":
            {
                source.Trackers.Enums.Set(FishOil.StartedQuest);
                source.SendOrangeBarMessage("Speak to Kamel about getting some oil for Antonio.");
                break;
            }

            case "fishoil_turnin":
            {
                var hasRequiredOil = source.Inventory.HasCount("Oil", 1);
                
                if (hasStage && (stage == FishOil.StartedQuest))
                    switch (hasRequiredOil)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Oil", 1, out _);
                            source.Trackers.Enums.Set(FishOil.None);
                            source.Trackers.TimedEvents.AddEvent("fishoilcd", TimeSpan.FromHours(24), true);

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
                                      50000);

                            ExperienceDistributionScript.GiveExp(source, 50000);
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
                                "You have come back empty handed? I guess it's okay for now. It's been creeking this long, I can live without.");
                            source.SendOrangeBarMessage("You think about the fish market.");

                            break;
                    }

                break;
            }
        }
    }
}