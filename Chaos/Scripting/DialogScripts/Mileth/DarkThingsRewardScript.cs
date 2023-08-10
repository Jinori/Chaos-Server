using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class DarkThingsRewardScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<DarkThingsRewardScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public DarkThingsRewardScript(Dialog subject, IItemFactory itemFactory, ILogger<DarkThingsRewardScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var spidersEyeCount = source.Inventory.CountOf("Spider's Eye");

        if (spidersEyeCount == 0)
        {
            Subject.Reply(source, "You have no Spider's Eye, which is what I need now.");

            return;
        }

        var amountToReward = spidersEyeCount * 1000;

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Entities.Experience, Topics.Entities.Dialog, Topics.Entities.Quest)
              .WithProperty(source).WithProperty(Subject)
              .LogInformation("{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from a quest", source.Name, amountToReward, amountToReward);
        
        source.TryGiveGold(amountToReward);
        ExperienceDistributionScript.GiveExp(source, amountToReward);
        source.Inventory.RemoveQuantity("Spider's Eye", spidersEyeCount, out _);
        source.TryGiveGamePoints(1);

        var message = $"You receive a gamepoint and {amountToReward} gold/exp!";
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, message);
        Subject.Reply(source, "Thank you for grabbing what I needed.");
    }
}