using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class DarkThingsRewardScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public DarkThingsRewardScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
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

        source.TryGiveGold(amountToReward);
        ExperienceDistributionScript.GiveExp(source, amountToReward);
        source.Inventory.RemoveQuantity("Spider's Eye", spidersEyeCount, out _);
        source.TryGiveGamePoints(1);

        var message = $"You receive a gamepoint and {amountToReward} gold/exp!";
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, message);
        Subject.Reply(source, "Thank you for grabbing what I needed.");
    }
}