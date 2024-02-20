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

namespace Chaos.Scripting.DialogScripts.Quests.Rucesion;

public class decoratingInnQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SpareAStickScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public decoratingInnQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out DecoratingInn stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "runa_initial":
            {
                
                var option = new DialogOption
                {
                    DialogKey = "decoratinginn_initial",
                    OptionText = "Decorating the Inn"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "decoratinginn_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("decoratingtheinncd", out var cdtime))
                {
                    Subject.Reply(source, $"I appreciate your efforts in decorating the inn. These flowers won't last long, please come back soon. (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }

                if (hasStage && stage == DecoratingInn.CompletedQuest)
                {
                    Subject.Reply(source, "Skip", "decoratinginn_returnstart");
                    return;
                }
                
                if (hasStage && stage == DecoratingInn.StartedPetunia)
                {
                    Subject.Reply(source, "Skip", "decoratinginn_return1");
                    return;
                }
                if (hasStage && stage == DecoratingInn.CompletedPetunia)
                {
                    Subject.Reply(source, "Skip", "decoratinginn_initial2");
                    return;
                }
                if (hasStage && stage == DecoratingInn.StartedGoldRose)
                {
                    Subject.Reply(source, "Skip", "decoratinginn_return1");
                    return;
                }
                if (hasStage && stage == DecoratingInn.CompletedGoldRose)
                {
                    Subject.Reply(source, "Skip", "decoratinginn_initial3");
                    return;
                }
                if (hasStage && stage == DecoratingInn.StartedPinkRose)
                {
                    Subject.Reply(source, "Skip", "decoratinginn_return1");
                    return;
                }
                break;
            }

            case "decoratinginn_startquest":
            {
                if (!hasStage || stage == DecoratingInn.None)
                {
                    Subject.Reply(source, "That's what I like to hear. See you soon!");
                    source.Trackers.Enums.Set(DecoratingInn.StartedPetunia);
                    source.SendOrangeBarMessage("Retrieve 3 Petunia for Maria.");
                    return;
                }
                if (hasStage && stage == DecoratingInn.CompletedPetunia)
                {
                    Subject.Reply(source, "Perfect. That gold rose will make a beautiful center piece.");
                    source.Trackers.Enums.Set(DecoratingInn.StartedGoldRose);
                    source.SendOrangeBarMessage("Retrieve 1 Gold Rose for Maria.");
                    return;
                }
                if (hasStage && stage == DecoratingInn.CompletedGoldRose)
                {
                    Subject.Reply(source, "Those Pink Roses will be the finishing touch to the baskets. Please hurry.");
                    source.Trackers.Enums.Set(DecoratingInn.StartedPinkRose);
                    source.SendOrangeBarMessage("Retrieve 3 Pink Rose for Maria.");
                    return;
                }
                
                break;
            }

            case "decoratinginn_turnin":
            {
                var hasRequiredPetunia = source.Inventory.HasCount("Petunia", 3);
                var hasRequiredGoldRose = source.Inventory.HasCount("Gold Rose", 1);
                var hasRequiredPinkRose = source.Inventory.HasCount("Pink Rose", 3);


                if (hasStage && stage == DecoratingInn.StartedPetunia)
                {
                    if (hasRequiredPetunia)
                    {
                        source.Inventory.RemoveQuantity("Petunia", 3, out _);
                        source.Trackers.Enums.Set(DecoratingInn.CompletedPetunia);

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

                        ExperienceDistributionScript.GiveExp(source, 5000);
                    }

                    if (!hasRequiredPetunia)
                    {
                        var petuniaCount = source.Inventory.CountOf("Petunia");

                        if (petuniaCount < 1)
                        {
                            Subject.Reply(source, "I don't see any Petunias. Are you sure you found them? I need three for a basket.");
                            return;
                        }
                        
                        Subject.Reply(source,
                            $"{petuniaCount} petunias won't be enough. I need at least three to make one basket for a room.");
                    }
                }
                
                if (hasStage && stage == DecoratingInn.StartedGoldRose)
                {
                    if (hasRequiredPetunia)
                    {
                        source.Inventory.RemoveQuantity("Gold Rose", 1, out _);
                        source.Trackers.Enums.Set(DecoratingInn.CompletedGoldRose);

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
                    }

                    if (!hasRequiredGoldRose)
                    {
                        var goldRoseCount = source.Inventory.CountOf("Gold Rose");

                        if (goldRoseCount < 1)
                        {
                            Subject.Reply(source, "I really need that Gold Rose for the basket, it is the center piece. Please go find me a Gold Rose.");
                            return;
                        }
                    }
                }
                
                if (hasStage && stage == DecoratingInn.StartedPinkRose)
                {
                    if (hasRequiredPinkRose)
                    {
                        source.Inventory.RemoveQuantity("Pink Rose", 3, out _);
                        source.Trackers.Enums.Set(DecoratingInn.CompletedQuest);
                        source.Trackers.TimedEvents.AddEvent("decoratingtheinncd", TimeSpan.FromHours(24), true);

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

                        ExperienceDistributionScript.GiveExp(source, 40000);
                        source.TryGiveGold(25000);
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
                    }

                    if (!hasRequiredPinkRose)
                    {
                        var pinkRoseCount = source.Inventory.CountOf("Pink Rose");

                        if (pinkRoseCount < 1)
                        {
                            Subject.Reply(source, "You don't have any Pink Rose. I need three for the basket.");
                            return;
                        }
                        
                        Subject.Reply(source,
                            $"{pinkRoseCount} pink roses won't be enough. I need at least three to make one basket for a room.");
                    }
                }

                if (hasStage && stage == DecoratingInn.CompletedQuest)
                {
                    var hasRequiredFlowers = source.Inventory.HasCount("Petunia", 3) 
                                             && source.Inventory.HasCount("Gold Rose", 1) 
                                             && source.Inventory.HasCount("Pink Rose", 3);

                    if (hasRequiredFlowers)
                    {
                     source.Inventory.RemoveQuantity("Pink Rose", 3, out _);
                     source.Inventory.RemoveQuantity("Gold Rose", 1, out _);
                     source.Inventory.RemoveQuantity("Pink Rose", 3, out _);
                     source.Trackers.TimedEvents.AddEvent("decoratingtheinncd", TimeSpan.FromHours(24), true);

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

                        ExperienceDistributionScript.GiveExp(source, 75000);
                        source.TryGiveGold(30000);
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
                    }

                    if (!hasRequiredFlowers)
                    {
                        var pinkRoseCount = source.Inventory.CountOf("Pink Rose");
                        var goldRoseCount = source.Inventory.CountOf("Gold Rose");
                        var petuniaCount = source.Inventory.CountOf("petunia");

                        if ((pinkRoseCount < 1) || (goldRoseCount < 1) || (petuniaCount < 1))
                        {
                            Subject.Reply(source, "You're missing all of one of the flowers I asked for. Remember I need three petunias, one gold rose, and three pink roses.");
                            return;
                        }
                        
                        Subject.Reply(source,
                            $"You have only brought me {petuniaCount} petunias, {goldRoseCount} gold roses, and {pinkRoseCount} pink roses. This is not enough for a basket. I really need three petunias, one gold rose, and three pink roses.");
                    }
                }

                break;
            }
        }
    }
}