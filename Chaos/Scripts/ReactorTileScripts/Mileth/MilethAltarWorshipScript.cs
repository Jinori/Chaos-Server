using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.ReactorTileScripts.Mileth
{
    public class MilethAltarWorshipScript : ReactorTileScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private IExperienceDistributionScript ExperienceDistributionScript{ get; set; }
        public MilethAltarWorshipScript(ReactorTile subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
            ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        }

        public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
        {
            var aisling = (Aisling)source;
            aisling.MapInstance.RemoveObject(groundItem);
            if (groundItem.Item.Template.BuyCost >= 1000 || groundItem.Item.Template.SellValue >= 1000)
            {
                ExperienceDistributionScript.GiveExp(aisling, 200);
                if (Randomizer.RollChance(10))
                {
                    List<string> randomMessages = new List<string>() { "The gods are pleased with your sacrifice.", "The item glows before dissolving into the altar.", "Good fortune the gods will grant." };
                    var random = new Random();
                    int index = random.Next(randomMessages.Count);
                    aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, randomMessages[index]);
                }
                if (Randomizer.RollChance(2))
                {
                    if (aisling.UserStatSheet.BaseClass.Equals(BaseClass.Priest))
                    {
                        aisling.Legend.AddOrAccumulate(new Objects.Legend.LegendMark("Mileth Altar Worshipper", "milethWorship", MarkIcon.Yay, MarkColor.Pink, 1, Time.GameTime.Now));
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
                        ExperienceDistributionScript.GiveExp(aisling, 100000);
                        aisling.TryGiveItems(ItemFactory.Create("amethystring"));
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(aisling, 40000);
                        aisling.TryGiveItems(ItemFactory.Create("emeraldring"));
                    }
                }
            }
        }
    }
}
