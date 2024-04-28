using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.ReactorTileScripts.Mileth;

public class MilethAltarWorshipScript : ReactorTileScriptBase
{
    private readonly IItemFactory _itemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public MilethAltarWorshipScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
    {
        _itemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        if (source is not Aisling aisling)
            return;

        aisling.MapInstance.RemoveEntity(groundItem);

        ExperienceDistributionScript.GiveExp(aisling, groundItem.Item.Template is { BuyCost: < 1000, SellValue: < 1000 } ? 25 : 200);

        if (IntegerRandomizer.RollChance(10))
        {
            var randomMessages = new List<string>
            {
                "The gods are pleased with your sacrifice.", "The item glows before dissolving into the altar.",
                "Good fortune the gods will grant."
            };

            var random = new Random();
            var index = random.Next(randomMessages.Count);
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, randomMessages[index]);
        }

        if (!IntegerRandomizer.RollChance(1))
            return;

        if (aisling.UserStatSheet.BaseClass.Equals(BaseClass.Priest))
        {
            aisling.Legend.AddOrAccumulate(
                new LegendMark(
                    "Mileth Altar Worshipper",
                    "milethWorship",
                    MarkIcon.Yay,
                    MarkColor.Pink,
                    1,
                    GameTime.Now));

            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
            ExperienceDistributionScript.GiveExp(aisling, 20000);
            aisling.GiveItemOrSendToBank(_itemFactory.Create("amethystring"));
        }
        else
        {
            ExperienceDistributionScript.GiveExp(aisling, 5000);
            aisling.GiveItemOrSendToBank(_itemFactory.Create("emeraldring"));
        }
    }
}