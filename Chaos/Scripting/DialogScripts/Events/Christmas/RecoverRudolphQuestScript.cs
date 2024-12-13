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

public class RecoverRudolphQuestScript(Dialog subject, ILogger<RecoverRudolphQuestScript> logger, IItemFactory itemFactory)
    : DialogScriptBase(subject)
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out Rudolph stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var seventyFivePercent = MathEx.GetPercentOf<int>(tnl, 75);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "santa_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "rudolph_initial",
                    OptionText = "Rudolph"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "rudolph_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("rudolphcd", out var cdtime))
                {
                    Subject.Reply(source, $"Rudolph is back home safely for now! {cdtime.Remaining.ToReadableString()}");

                    return;
                }

                if (hasStage && (stage == Rudolph.StartedQuest))
                    Subject.Reply(source, "Skip", "rudolph_return1");

                break;
            }

            case "rudolph_start1":
            {
                source.Trackers.Enums.Set(Rudolph.StartedQuest);
                source.SendOrangeBarMessage("Find Rudolph!");

                break;
            }

            case "rudolph_turnin":
            {
                var hasRequiredRudolph = source.Inventory.HasCount("Rudolph", 1);

                if (hasStage && (stage == Rudolph.StartedQuest))
                    switch (hasRequiredRudolph)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Rudolph", 1, out _);
                            source.Trackers.Enums.Set(Rudolph.None);
                            source.Trackers.TimedEvents.AddEvent("rudolphcd", TimeSpan.FromHours(6), true);

                            var expRewarded = 0;

                            if (source.UserStatSheet.Level == 99)
                                expRewarded = 50000000;

                            if (source.UserStatSheet.Level < 99)
                                expRewarded = seventyFivePercent;

                            ExperienceDistributionScript.GiveExp(source, expRewarded);

                            logger.WithTopics(
                                      [
                                          Topics.Entities.Aisling,
                                          Topics.Entities.Gold,
                                          Topics.Entities.Experience,
                                          Topics.Entities.Dialog,
                                          Topics.Entities.Quest
                                      ])
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@ExpAmount} exp from returning Rudolph.",
                                      source.Name,
                                      expRewarded);

                            source.TryGiveGamePoints(10);
                            var present = ItemFactory.Create("present");
                            source.GiveItemOrSendToBank(present);

                            if (IntegerRandomizer.RollChance(5))
                            {
                                source.Legend.AddOrAccumulate(
                                    new LegendMark(
                                        "Returned Rudolph to Santa",
                                        "rudolphReturned",
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
                            Subject.Reply(source, "I don't see Rudolph with you. Did he escape you?");

                            break;
                        }
                    }

                break;
            }
        }
    }
}