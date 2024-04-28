using Chaos.Common.Definitions;
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

namespace Chaos.Scripting.DialogScripts.Quests.KarloposQuest;

public class QueenOctopusQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<QueenOctopusQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public QueenOctopusQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<QueenOctopusQuestScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "maria_initial":
            {
                if (!hasStage || (stage == QueenOctopusQuest.None))
                {
                    if (source.UserStatSheet.Level is <= 41 or >= 72)
                        return;
                    
                    var option = new DialogOption
                    {
                        DialogKey = "QueenOctopus_start",
                        OptionText = "Queen Octopus?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                }

                if (hasStage)
                {
                    if (source.UserStatSheet.Level is <= 41 or >= 72)
                        return;
                    
                    var option = new DialogOption
                    {
                        DialogKey = "QueenOctopus_start",
                        OptionText = "Queen Octopus?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                }
            }

                break;

            case "queenoctopus_start":
            {
                switch (stage)
                {
                    case QueenOctopusQuest.Liver:
                        Subject.Reply(source, "skip", "queenoctopus_liver");

                        return;
                    case QueenOctopusQuest.Pendant:
                        Subject.Reply(source, "skip", "queenoctopus_liver2");

                        return;
                    case QueenOctopusQuest.Pendant3:
                        Subject.Reply(source, "skip", "queenoctopus_Queen");

                        return;

                    case QueenOctopusQuest.QueenKilled:
                        Subject.Reply(source, "skip", "queenoctopus_Queenkilled");

                        return;

                    case QueenOctopusQuest.Complete:
                        Subject.Reply(source, "Welcome Back. Please make yourself comfortable.");
                        return;
                }

                break;
            }

            case "bret_initial":
            {
                if (stage == QueenOctopusQuest.Pendant)
                {
                    Subject.Reply(source, "skip", "queenoctopus_pendant1");

                    return;
                }

                if (stage == QueenOctopusQuest.Pendant2)
                    Subject.Reply(source, "Be safe.", "close");
            }

                break;

            case "queenoctopus_start2":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.Liver);
            }

                break;
            case "queenoctopus_liver1":
            {
                if (!source.Inventory.RemoveQuantity("liver", 5))
                {
                    Subject.Reply(
                        source,
                        "Please collect 5 liver for me.");

                    return;
                }

                var redpearl = ItemFactory.Create("redpearl");

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Experience,
                          Topics.Entities.Item,
                          Topics.Entities.Dialog,
                          Topics.Entities.Quest)
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation(
                          "{@AislingName} has received {@ExpAmount} exp and the item {@ItemName}",
                          source.Name,
                          200000,
                          redpearl.DisplayName);

                ExperienceDistributionScript.GiveExp(source, 200000);
                source.SendOrangeBarMessage("You received 200,000 experience!");
                source.Trackers.Enums.Set(QueenOctopusQuest.Pendant);
                source.GiveItemOrSendToBank(redpearl);

                Subject.Reply(
                    source,
                    "Oh wow! That was fast. A deal is a deal. Here is the Red Pearl. The only other clue I can remember is him mentioning a secret entrance in Karlopos Island North. If you see my brother out there please tell him to come home. I'm worried about him.",
                    "QueenOctopus_liver2");
            }

                break;

            case "queenoctopus_pendant6":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.Pendant2);
            }

                break;
            case "queenoctopus_queen2":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.QueenKilled);
            }

                break;

            case "queenoctopus_queenkilled3":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.Complete);

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Experience,
                          Topics.Entities.Dialog,
                          Topics.Entities.Quest)
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} has received {@ExpAmount} exp", source.Name, 500000);

                ExperienceDistributionScript.GiveExp(source, 500000);
                source.SendOrangeBarMessage("You received 500,000 experience!");

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Killed the Queen Octopus of Karlopos Island",
                        "QueenOctopus",
                        MarkIcon.Heart,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));
            }

                break;
        }
    }
}