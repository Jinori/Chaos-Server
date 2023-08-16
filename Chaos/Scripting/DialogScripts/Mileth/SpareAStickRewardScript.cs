using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
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
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class SpareAStickRewardScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SpareAStickRewardScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public SpareAStickRewardScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickRewardScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var branchCount = source.Inventory.CountOf("Branch");

        if (source.Trackers.Flags.HasFlag(QuestFlag1.GatheringSticks))
        {
            if (branchCount >= 6)
            {
                Subject.Reply(source, "Excellent! You'll make a fine spark. Now, go and find your way.");
                source.Trackers.Flags.RemoveFlag(QuestFlag1.GatheringSticks);
                source.Trackers.Flags.AddFlag(QuestFlag1.SpareAStickComplete);

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
                          1000,
                          2500);

                ExperienceDistributionScript.GiveExp(source, 2500);
                source.TryGiveGold(1000);
                source.TryGiveGamePoints(5);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "5 Gamepoints, 1000 gold, and 2500 Exp Rewarded!");

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

                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
                }

                source.Inventory.RemoveQuantity("branch", 6, out _);
                var stick = ItemFactory.Create("Stick");
                var shield = ItemFactory.Create("woodenshield");
                source.TryGiveItems(stick, shield);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.SpareAStick);
            }
            else
                Subject.Reply(
                    source,
                    branchCount == 0
                        ? "What? No branches?! You haven't even tried."
                        : $"Only {branchCount} branches.. you need six! Go get the rest.");
        }
    }
}