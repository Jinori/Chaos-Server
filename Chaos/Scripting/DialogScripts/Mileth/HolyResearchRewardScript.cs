using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class HolyResearchRewardScript : DialogScriptBase
{
    private readonly IExperienceDistributionScript ExperienceDistributionScript;
    private readonly ILogger<HolyResearchRewardScript> Logger;

    public HolyResearchRewardScript(Dialog subject, IExperienceDistributionScript experienceDistributionScript, ILogger<HolyResearchRewardScript> logger)
        : base(subject)
    {
        ExperienceDistributionScript = experienceDistributionScript;
        Logger = logger;
    }

    public override void OnDisplaying(Aisling source)
    {
        var rawWaxCount = source.Inventory.CountOf("Raw Wax");

        if (rawWaxCount == 0)
        {
            Subject.Reply(source, "You have no Raw Wax, which is what I need now.");

            return;
        }

        var amountToReward = rawWaxCount * 1000;
        
        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Entities.Experience, Topics.Entities.Dialog, Topics.Entities.Quest)
              .WithProperty(source).WithProperty(Subject)
              .LogInformation("{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from a quest", source.Name, amountToReward, amountToReward);
        
        source.TryGiveGold(amountToReward);
        ExperienceDistributionScript.GiveExp(source, amountToReward);
        source.Inventory.RemoveQuantity("Raw Wax", rawWaxCount);
        source.TryGiveGamePoints(1);
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive a game point and {amountToReward} gold/exp!");
        Subject.Reply(source, "Thank you for grabbing what I needed.");
    }
}