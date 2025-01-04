using Chaos.Common.Definitions;
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
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Piet;

public class PietWoodQuestScript : DialogScriptBase
{
    private readonly ILogger<PietWoodQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public PietWoodQuestScript(Dialog subject, ILogger<PietWoodQuestScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out PietWood stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            
            case "saskia_initial":
            {
                
                if (source.UserStatSheet.Level < 11)
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "pietwood_initial",
                    OptionText = "Burning Wood"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "pietwood_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("pietwoodcd", out var cdtime))
                {
                    Subject.Reply(source, $"My Inn is so warm now, thank you Aisling. I am so comfortable now. I'll probably need more wood soon, come back in (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                if (hasStage && stage == PietWood.StartedQuest)
                {
                    Subject.Reply(source, "Skip", "pietwood_return");
                }
                break;
            }

            case "pietwood_start2":
            {
                source.Trackers.Enums.Set(PietWood.StartedQuest);
                source.SendOrangeBarMessage("Gather Wood from trents for Saskia's Inn.");
                break;
            }

            case "pietwood_turnin":
            {
                var hasRequiredWolfFurs = source.Inventory.HasCount("Trent Wood", 20);
                
                if (hasStage && (stage == PietWood.StartedQuest))
                {
                    switch (hasRequiredWolfFurs)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Trent Wood", 20, out _);
                            source.Trackers.Enums.Set(PietWood.None);
                            source.Trackers.TimedEvents.AddEvent("pietwoodcd", TimeSpan.FromHours(22), true);

                            Logger.WithTopics(
                                      [Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest])
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from a quest",
                                      source.Name,
                                      5000,
                                      20000);

                            ExperienceDistributionScript.GiveExp(source, 20000);
                            source.TryGiveGold(5000);
                            source.TryGiveGamePoints(5);

                            if (IntegerRandomizer.RollChance(8))
                            {
                                source.Legend.AddOrAccumulate(
                                    new LegendMark(
                                        "Loved by Piet Mundanes",
                                        "pietLoved",
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
                            var trentWood = source.Inventory.CountOf("Trent Wood");

                            if (trentWood < 1)
                            {
                                Subject.Reply(source, "Where is the Trent Wood at? Are you too weak?");
                                return;
                            }
                        
                            Subject.Reply(source,
                                $"I understand trent wood can be difficult to find... you only have {trentWood} Trent Wood. That won't be enough, I usually use 20 Trent Wood a day to keep my Inn warm!");

                            break;
                        }
                    }
                }

                break;
            }
        }
    }
}