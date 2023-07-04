using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class HolyResearchRewardScript : DialogScriptBase
{
    private readonly IExperienceDistributionScript ExperienceDistributionScript;

    public HolyResearchRewardScript(Dialog subject, IExperienceDistributionScript experienceDistributionScript)
        : base(subject) =>
        ExperienceDistributionScript = experienceDistributionScript;

    public override void OnDisplaying(Aisling source)
    {
        var rawWaxCount = source.Inventory.CountOf("Raw Wax");

        if (rawWaxCount == 0)
        {
            Subject.Reply(source, "You have no Raw Wax, which is what I need now.");

            return;
        }

        var amountToReward = rawWaxCount * 1000;
        source.TryGiveGold(amountToReward);
        ExperienceDistributionScript.GiveExp(source, amountToReward);
        source.Inventory.RemoveQuantity("Raw Wax", rawWaxCount);
        source.TryGiveGamePoints(1);
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive a game point and {amountToReward} gold/exp!");
        Subject.Reply(source, "Thank you for grabbing what I needed.");
    }
}