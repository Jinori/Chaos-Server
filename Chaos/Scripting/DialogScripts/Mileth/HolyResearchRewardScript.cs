using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class HolyResearchRewardScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public HolyResearchRewardScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Inventory.CountOf("Raw Wax") == 0)
            Subject.Text = "You have no Raw Wax, which is what I need now.";

        if (source.Inventory.CountOf("Raw Wax") >= 1)
        {
            var amountToReward = source.Inventory.CountOf("Raw Wax") * 1000;
            source.TryGiveGold(amountToReward);
            ExperienceDistributionScript.GiveExp(source, amountToReward);
            source.Inventory.RemoveQuantity("Raw Wax", source.Inventory.CountOf("Raw Wax"), out _);
            source.TryGiveGamePoints(1);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive a gamepoint and {amountToReward} gold/exp!");
            Subject.Text = "Thank you for grabbing what I needed.";
        }
    }
}