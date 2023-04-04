using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
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
        if (source.Inventory.CountOf("Spider's Eye") == 0)
            Subject.Reply(source, "You have no Spider's Eye, which is what I need now.");

        if (source.Inventory.CountOf("Spider's Eye") >= 1)
        {
            var amountToReward = source.Inventory.CountOf("Spider's Eye") * 1000;
            source.TryGiveGold(amountToReward);
            ExperienceDistributionScript.GiveExp(source, amountToReward);
            source.Inventory.RemoveQuantity("Spider's Eye", source.Inventory.CountOf("Spider's Eye"), out _);
            source.TryGiveGamePoints(1);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive a gamepoint and {amountToReward} gold/exp!");
            Subject.Reply(source, "Thank you for grabbing what I needed.");
        }
    }
}