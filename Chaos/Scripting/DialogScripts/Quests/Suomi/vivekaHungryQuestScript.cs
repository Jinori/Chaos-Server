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
using Chaos.Scripting.DialogScripts.Mileth;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Suomi;

public class VivekaHungryQuestScript : DialogScriptBase
{
    private readonly ILogger<SpareAStickScript> Logger;

    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public VivekaHungryQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickScript> logger)
        : base(subject)
    {
        Logger = logger;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out HungryViveka stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "viveka_initial":
            {
                if (hasStage && stage == HungryViveka.CompletedCherryPie)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "vivekahungry_initial2",
                        OptionText = "Oh, it's you!"
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                    return;
                }

                if (hasStage && stage == HungryViveka.StartedCherryPie)
                {
                    var option2 = new DialogOption
                    {
                        DialogKey = "vivekahungry_initial",
                        OptionText = "I'm really hungry."
                    };

                    if (!Subject.HasOption(option2.OptionText))
                        Subject.Options.Insert(0, option2);
                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "vivekahungry_initial",
                    OptionText = "Hey, do you have a moment?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "vivekahungry_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("vivekahungrycd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I cannot express how much that cherry pie meant to me. I would eat another if you brought one in (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }

                if (hasStage && stage == HungryViveka.StartedCherryPie)
                {
                    Subject.Reply(source, "Skip", "vivekaHungry_turnin");
                }

                break;
            }

            case "vivekahungry_initial2":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("vivekahungrycd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I cannot express how much that cherry pie meant to me. I would eat another if you brought one in (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }

                break;
            }

            case "vivekahungry_startquest3":
            {
                if (!hasStage || stage == HungryViveka.None)
                {
                    source.Trackers.Enums.Set(HungryViveka.StartedCherryPie);
                    source.SendOrangeBarMessage("Bring Viveka something to eat that has cherries.");
                }

                break;
            }

            case "vivekahungry_turnin2":
            {
                var hasRequiredCherryPie = source.Inventory.HasCount("Cherry Pie", 1);

                if (hasStage && stage == HungryViveka.StartedCherryPie)
                {
                    if (hasRequiredCherryPie)
                    {
                        source.Inventory.RemoveQuantity("Cherry Pie", 1, out _);
                        source.Trackers.Enums.Set(HungryViveka.CompletedCherryPie);
                        source.Trackers.TimedEvents.AddEvent("vivekahungrycd", TimeSpan.FromHours(22), true);
                        var wine1 = ItemFactory.Create("wine");
                        var wine2 = ItemFactory.Create("wine");
                        source.GiveItemOrSendToBank(wine1);
                        source.GiveItemOrSendToBank(wine2);

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

                        ExperienceDistributionScript.GiveExp(source, 25000);

                        if (IntegerRandomizer.RollChance(8))
                        {
                            source.Legend.AddOrAccumulate(
                                new LegendMark(
                                    "Loved by Suomi Mundanes",
                                    "SuomiLoved",
                                    MarkIcon.Heart,
                                    MarkColor.Blue,
                                    1,
                                    GameTime.Now));

                            source.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                                "You received a unique legend mark!");
                        }
                    }
                }

                if (!hasRequiredCherryPie)
                {
                    var cherryCount = source.Inventory.HasCount("Cherry", 1);

                    if (cherryCount)
                    {
                        Subject.Reply(source,
                            "I'm not eating just cherries from the garden! Gross. Go give those back to the farmer or do something good with them!");
                        return;
                    }

                    Subject.Reply(source,
                        $"That isn't much of a surprise. Nothing? Come back when you have my surprise sweety.");
                }

                break;
            }

            case "vivekahungry_turnin3":
            {
                var hasRequiredCherryPie = source.Inventory.HasCount("Cherry Pie", 1);

                if (hasStage && stage == HungryViveka.CompletedCherryPie)
                {
                    if (hasRequiredCherryPie)
                    {
                        source.Inventory.RemoveQuantity("Cherry Pie", 1, out _);
                        source.Trackers.TimedEvents.AddEvent("vivekahungrycd", TimeSpan.FromHours(22), true);

                        Logger.WithTopics(
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
                                5000);

                        ExperienceDistributionScript.GiveExp(source, 5000);
                    }


                    if (IntegerRandomizer.RollChance(8))
                    {
                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Loved by Suomi Mundanes",
                                "SuomiLoved",
                                MarkIcon.Heart,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));

                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                            "You received a unique legend mark!");
                    }

                    if (!hasRequiredCherryPie)
                    {
                        Subject.Reply(source, "Well where is it? Did you forget my pie? I don't have time for this. *Viveka walks off and continues to work*");
                    }
                }

                break;
            }
        }
    }
}