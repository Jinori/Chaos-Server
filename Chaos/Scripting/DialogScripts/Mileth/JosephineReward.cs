using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class JosephineRewardScript : DialogScriptBase
{
    private readonly ILogger<JosephineRewardScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public JosephineRewardScript(Dialog subject, ILogger<JosephineRewardScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage stage) && (stage == RionaTutorialQuestStage.StartedBeautyShop))
        {
            source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedBeautyShop);
            Logger.WithTopics(
                      [Topics.Entities.Aisling,
                      Topics.Entities.Gold,
                      Topics.Entities.Experience,
                      Topics.Entities.Dialog,
                      Topics.Entities.Quest])
                  .WithProperty(source)
                  .WithProperty(Subject)
                  .LogInformation(
                      "{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from a quest",
                      source.Name,
                      1000,
                      1000);

            ExperienceDistributionScript.GiveExp(source, 1000);
            source.TryGiveGold(1000);
            source.TryGiveGamePoints(5);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've received 1000g, 1000exp and 5 game points!");
            Subject.Reply(source, "Riona sent you? I do have her dye, Let her know for me please. Hey, Are you interested in a hair style?", "josephine_initial");
        }
    }
}