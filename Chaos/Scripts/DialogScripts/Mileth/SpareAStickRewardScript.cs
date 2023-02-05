using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Mileth;

public class SpareAStickRewardScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public SpareAStickRewardScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.GatheringSticks) && (source.Inventory.CountOf("Branch") >= 6))
        {
            Subject.Text = "Excellent! You'll make a fine spark. Now, go and find your way.";
            source.Flags.RemoveFlag(QuestFlag1.GatheringSticks);
            source.Flags.AddFlag(QuestFlag1.SpareAStickComplete);
            ExperienceDistributionScript.GiveExp(source, 2500);
            source.TryGiveGamePoints(5);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and 2500 exp!");
            
            
            if (Randomizer.RollChance(8))
            {
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Loved by Mileth Mundanes",
                        "milethLoved",
                        MarkIcon.Heart,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));

                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
            }

            source.Inventory.RemoveQuantity("branch", 6, out _);
            var stick = ItemFactory.Create("Stick");
            source.TryGiveItem(stick);
        }

        if (source.Flags.HasFlag(QuestFlag1.GatheringSticks) && (source.Inventory.CountOf("Branch") < 6))
        {
            if (source.Inventory.CountOf("Branch") == 0)
                Subject.Text = "What? No branches?! You haven't even tried.";
            else
            {
                var count = source.Inventory.CountOf("branch");
                Subject.Text = $"Only {count} branches.. you need six! Go get the rest.";
            }
        }
    }
}