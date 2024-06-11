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

namespace Chaos.Scripting.DialogScripts.Quests.Piet;

public class PietWerewolfScript : DialogScriptBase
{
    private readonly ILogger<PietWerewolfScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public PietWerewolfScript(Dialog subject, ILogger<PietWerewolfScript> logger)
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
            case "werewolf_initial2":
            {
                if (!source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledWerewolf))
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "werewolf_initial3",
                    OptionText = "Werewolf Cure"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "werewolf_initial3":
            {
                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed))
                {
                    Subject.Reply(source, "Skip", "werewolf_initial4");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower)
                    || source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)
                    || source.Trackers.Enums.HasValue(WerewolfOfPiet.EnteredSWpath)
                    || source.Trackers.Enums.HasValue(WerewolfOfPiet.SpawnedWerewolf))
                {
                    Subject.Reply(source, "Skip", "werewolf_return");
                    return;
                }

                break;
            }

            case "werewolf_return":
            {
                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower))
                {
                    Subject.Reply(source, "Skip", "werewolf_turnin1");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.EnteredSWpath))
                {
                    Subject.Reply(source, "So you found the Werewolf's Woods, that's good. Get through the woods and find that flower. We will need it for the cure.");
                    source.SendOrangeBarMessage("Return to the woods and find the flower.");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpawnedWerewolf))
                {
                    Subject.Reply(source, "Looks like you took a beating, that werewolf is tough. You have to go back and get that flower.");
                    source.SendOrangeBarMessage("Kill the Werewolf and retrieve the flower.");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard))
                {
                    Subject.Reply(source, "Why are you back here? Go search Shinewood Forest for the Werewolf's Woods and retrieve the flower we need for the cure.");
                    source.SendOrangeBarMessage("Search Shinewood Forest for Werewolf's Woods.");
                    return;
                }
                break;
            }

            case "werewolf_initial7":
            {
                source.Trackers.Enums.Set(WerewolfOfPiet.SpokeToWizard);
                source.SendOrangeBarMessage("Search Shinewood Forest for the Werewolf's Woods.");
                break;
            }

            case "werewolf_start2":
            {
                source.Trackers.Enums.Set(WerewolfOfPiet.StartedQuest);
                source.SendOrangeBarMessage("Search for the Werewolf at night.");
                break;
            }

            case "werewolf_turnin":
            {
                var hasRequiredBlueFlower = source.Inventory.HasCount("Blue Flower", 1);
                
                    switch (hasRequiredBlueFlower)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Blue Flower", 1, out _);
                            source.Trackers.Enums.Set(WerewolfOfPiet.ReceivedCure);

                            Logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from Werewolf Quest",
                                      source.Name,
                                      5000,
                                      20000);

                            ExperienceDistributionScript.GiveExp(source, 600000);
                            source.TryGiveGold(75000);
                            source.TryGiveGamePoints(10);

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
                    }
                break;
            }
        }
    }
}