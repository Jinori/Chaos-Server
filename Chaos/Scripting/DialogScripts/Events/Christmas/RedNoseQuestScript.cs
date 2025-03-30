using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
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

namespace Chaos.Scripting.DialogScripts.Events.Christmas;

public class RedNoseQuestScript(Dialog subject, ILogger<RedNose> logger, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out RedNose stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var fiftyPercent = MathEx.GetPercentOf<int>(tnl, 50);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "elf3_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "rednose_initial",
                    OptionText = "Red Nose"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "rednose_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("rednosecd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"We appreciate your services Aisling, those Red Noses are going to be so bright for Santa! However, we will accept extras in {cdtime.Remaining.ToReadableString()}");

                    return;
                }

                if (hasStage && (stage == RedNose.StartedQuest))
                    Subject.Reply(source, "Skip", "rednose_return1");

                break;
            }

            case "rednose_start1":
            {
                source.Trackers.Enums.Set(RedNose.StartedQuest);
                source.SendOrangeBarMessage("Kill monsters in Mount Merry to retrieve 20 Red Noses.");

                break;
            }

            case "rednose_turnin":
            {
                var hasRequiredRedNose = source.Inventory.HasCount("Red Nose", 20);

                if (hasStage && (stage == RedNose.StartedQuest))
                    switch (hasRequiredRedNose)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Red Nose", 20, out _);
                            source.Trackers.Enums.Set(RedNose.None);
                            source.Trackers.TimedEvents.AddEvent("rednosecd", TimeSpan.FromHours(6), true);

                            var expRewarded = 0;

                            if (source.UserStatSheet.Level == 99)
                                expRewarded = 50000000;

                            if (source.UserStatSheet.Level < 99)
                                expRewarded = fiftyPercent;

                            ExperienceDistributionScript.GiveExp(source, expRewarded);

                            logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@ExpAmount} exp from returning Rudolph.",
                                      source.Name,
                                      expRewarded);

                            source.TryGiveGamePoints(10);
                            var present = ItemFactory.Create("present");
                            source.GiveItemOrSendToBank(present);

                            if (IntegerRandomizer.RollChance(10))
                            {
                                source.Legend.AddOrAccumulate(
                                    new LegendMark(
                                        "Loved by North Pole Elves",
                                        "northPoleLoved",
                                        MarkIcon.Heart,
                                        MarkColor.Blue,
                                        1,
                                        GameTime.Now));
                                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
                            }

                            break;
                        }
                        case false:
                        {
                            var redNoseCount = source.Inventory.CountOf("Red Nose");

                            if (redNoseCount < 1)
                            {
                                Subject.Reply(
                                    source,
                                    "There's not even one red nose in your pocket! Please go fight those monsters for our red noses!");

                                return;
                            }

                            Subject.Reply(
                                source,
                                $"There's not enough red noses here, you only have {redNoseCount}. I need at least 20 red noses for the reindeer.");

                            break;
                        }
                    }

                break;
            }
        }
    }
}