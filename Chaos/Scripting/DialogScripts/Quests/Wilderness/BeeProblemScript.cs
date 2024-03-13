using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Quests.Wilderness;

public class BeeProblemScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BeeProblemScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public BeeProblemScript(Dialog subject, IItemFactory itemFactory, ILogger<BeeProblemScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out BeeProblem stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "talula_initial":
            {
                if (source.UserStatSheet.Level >= 16)
                    Subject.Reply(source, "Skip", "talula_initial3");
                
                if (!hasStage || stage == BeeProblem.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "talula_bee1",
                        OptionText = "Having a bee problem?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (hasStage && stage == BeeProblem.Started)
                    Subject.Reply(source, "Skip", "talula_initial2");
                
                if (hasStage && stage == BeeProblem.Completed)
                    Subject.Reply(source, "Skip", "talula_initial3");

                break;
            }

            case "talula_bee2":
            {
                source.Trackers.Enums.Set(BeeProblem.Started);
            }

                break;

            case "talula_bee3":
            {
                if (!source.Trackers.Counters.TryGetValue("wilderness_Bee", out var value) || (value < 5))
                {
                    Subject.Reply(source, "I can still hear them buzzing around.");
                    source.SendOrangeBarMessage("You need to kill 5 bees.");

                    return;
                }

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
                          1500,
                          "Wind Belt");

                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 1500);
                source.TryGiveItems(ItemFactory.Create("windbelt"));
                source.SendOrangeBarMessage("1500 Exp and Wind Belt Rewarded!");
                source.Trackers.Counters.Remove("wilderness_bee", out _);
                source.Trackers.Enums.Set(BeeProblem.Completed);
            }

                break;
        }
    }
}