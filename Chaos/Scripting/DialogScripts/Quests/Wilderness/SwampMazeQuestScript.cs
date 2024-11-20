using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Wilderness;

public class SwampMazeQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SwampMazeQuest> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public SwampMazeQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<SwampMazeQuest> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out SwampMazeQuest stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "koda_initial":

                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Reply(source, "skip", "koda_initiallow");

                    return;
                }

                if (stage == SwampMazeQuest.Start)
                {
                    Subject.Reply(source, "skip", "koda_initial2");

                    return;
                }

                if (stage == SwampMazeQuest.Complete)
                    Subject.Reply(source, "skip", "koda_initial3");

                break;

            case "joda_initial2":

                if (stage == SwampMazeQuest.Start)
                {
                    Subject.Reply(source, "skip", "joda_initial");

                    return;
                }

                if (stage == SwampMazeQuest.Complete)
                    Subject.Reply(source, "skip", "joda_initial3");

                break;

            case "koda_bye":
            {
                source.Trackers.Enums.Set(SwampMazeQuest.Start);
                source.SendOrangeBarMessage("Koda seemed worried about his brother.");
            }

                break;

            case "joda_quest5":
            {
                if (stage == SwampMazeQuest.Start)
                {
                    source.Trackers.Enums.Set(SwampMazeQuest.Complete);

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Item,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation(
                              "{@AislingName} has received {@ExpAmount} exp from a quest and the item {@ItemName}",
                              source.Name,
                              25000,
                              "Mushroom Hat");

                    source.GiveItemOrSendToBank(ItemFactory.Create("mushroomhat"));
                    ExperienceDistributionScript.GiveExp(source, 25000);
                    source.SendOrangeBarMessage("You receive 25000 exp and a Mushroom Hat!");
                }
            }

                break;
        }
    }
}