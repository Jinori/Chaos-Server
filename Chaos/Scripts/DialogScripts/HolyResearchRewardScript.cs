using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class HolyResearchRewardScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;

        public HolyResearchRewardScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Inventory.CountOf("Raw Wax") == 0)
            {
                Subject.Text = "You have no Raw Wax, which is what I need now.";
            }
            if (source.Inventory.CountOf("Raw Wax") >= 1)
            {
                int amountToReward = source.Inventory.CountOf("Raw Wax") * 1000;
                source.TryGiveGold(amountToReward);
                source.GiveExp(amountToReward);
                source.Inventory.RemoveQuantity("Raw Wax", source.Inventory.CountOf("Raw Wax"), out _);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive {amountToReward} gold and exp!");
                Subject.Text = "Thank you for grabbing what I needed.";
            }
        }
    }
}
