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
using Chaos.Scripting.DialogScripts.Mileth;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Abel;

public class SharpestBladeQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SpareAStickScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public SharpestBladeQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out SharpestBlade stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "vidar_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "sharpestblade_initial",
                    OptionText = "Blade Material"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "sharpestblade_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("sharpestbladecd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"Those brawlfish scales will make perfect blades. I'm still working on them now, come back in (({cdtime.Remaining.ToReadableString()}))");

                    return;
                }

                if (hasStage && (stage == SharpestBlade.StartedQuest))
                    Subject.Reply(source, "Skip", "sharpestblade_return");

                break;
            }

            case "sharpestblade_start2":
            {
                source.Trackers.Enums.Set(SharpestBlade.StartedQuest);
                source.SendOrangeBarMessage("Retrieve 5 Brawlfish Scales for Vidar.");

                break;
            }

            case "sharpestblade_turnin":
            {
                var hasRequiredBrawlfishScale = source.Inventory.HasCount("Brawlfish's Scale", 5);

                if (hasStage && (stage == SharpestBlade.StartedQuest))
                {
                    if (hasRequiredBrawlfishScale)
                    {
                        source.Inventory.RemoveQuantity("Brawlfish's Scale", 5, out _);
                        source.Trackers.Enums.Set(SharpestBlade.None);
                        source.Trackers.TimedEvents.AddEvent("sharpestbladecd", TimeSpan.FromHours(22), true);

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
                                  25000,
                                  75000);

                        ExperienceDistributionScript.GiveExp(source, 75000);
                        source.TryGiveGold(25000);
                        source.TryGiveGamePoints(5);

                        if (IntegerRandomizer.RollChance(8))
                        {
                            source.Legend.AddOrAccumulate(
                                new LegendMark(
                                    "Loved by Abel Mundanes",
                                    "abelLoved",
                                    MarkIcon.Heart,
                                    MarkColor.Blue,
                                    1,
                                    GameTime.Now));

                            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
                        }
                    }

                    if (!hasRequiredBrawlfishScale)
                    {
                        var brawlfishCount = source.Inventory.CountOf("Brawlfish's Scale");

                        if (brawlfishCount < 1)
                        {
                            Subject.Reply(source, "You don't even have one! Please go get those Brawlfish's Scales.");

                            return;
                        }

                        Subject.Reply(
                            source,
                            $"You only have {brawlfishCount} Brawlfish's Scales. I need at least 5 incase I break a few.");
                    }
                }

                break;
            }
        }
    }
}