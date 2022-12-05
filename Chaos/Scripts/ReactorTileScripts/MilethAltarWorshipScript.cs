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

namespace Chaos.Scripts.ReactorTileScripts
{
    public class MilethAltarWorshipScript : ReactorTileScriptBase
    {
        private readonly IItemFactory ItemFactory;
        public MilethAltarWorshipScript(ReactorTile subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
        {
            var aisling = source as Aisling;
            aisling?.MapInstance.RemoveObject(groundItem);
            if (groundItem.Item.Template.BuyCost >= 1000 || groundItem.Item.Template.SellValue >= 1000)
            {
                aisling?.GiveExp(200);
                if (Randomizer.RollChance(10))
                {
                    List<string> randomMessages = new List<string>() { "The gods are pleased with your sacrifice.", "The item glows before dissolving into the altar.", "Good fortune the gods will grant." };
                    var random = new Random();
                    int index = random.Next(randomMessages.Count);
                    aisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, randomMessages[index]);
                }
                if (Randomizer.RollChance(2))
                {
                    if (aisling!.UserStatSheet.BaseClass.Equals(BaseClass.Priest))
                    {
                        aisling?.Legend.AddOrAccumulate(new Objects.Legend.LegendMark("Mileth Altar Worshipper", "milethWorship", MarkIcon.Yay, MarkColor.Pink, 1, Time.GameTime.Now));
                        aisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
                        aisling?.GiveExp(100000);
                        aisling?.TryGiveItems(ItemFactory.Create("amethystring"));
                    }
                    else
                    {
                        aisling?.GiveExp(40000);
                        aisling?.TryGiveItems(ItemFactory.Create("emeraldring"));
                    }
                }
            }
        }
    }
}
