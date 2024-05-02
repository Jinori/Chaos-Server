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

namespace Chaos.Scripting.DialogScripts.Mileth;

public class SpareAStickScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SpareAStickScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    public SpareAStickScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage stage);
        
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "callo_initial":
            {
                if (stage != RionaTutorialQuestStage.StartedSpareAStick && stage != RionaTutorialQuestStage.CompletedRatQuest)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "callo_spareastickinitial",
                    OptionText = "Spare a Stick"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "callo_spareastickgather":
            {
                if (stage != RionaTutorialQuestStage.CompletedRatQuest)
                    return;

                source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedSpareAStick);

                break;
            }

            case "callo_spareastickending":
            {
                var branchCount = source.Inventory.HasCount("Branch", 6);

        if (hasStage && stage == RionaTutorialQuestStage.StartedSpareAStick)
        {
            if (branchCount)
            {
                Subject.Reply(source, "Excellent! You'll make a fine spark. Now, go find your way.");
                source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedSpareAStick);

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
                source.GiveItemOrSendToBank(stick);
                source.GiveItemOrSendToBank(shield);
                source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.SpareAStick);
            }
            else
                Subject.Reply(
                    source,
                    branchCount
                        ? "What? No branches?! You haven't even tried."
                        : $"Only {branchCount} branches.. you need six! Go get the rest.");
        }
                break;
            }
        }
    }
}