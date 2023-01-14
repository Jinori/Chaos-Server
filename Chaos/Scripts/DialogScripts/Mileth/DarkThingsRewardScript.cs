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
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts.Mileth
{
    public class DarkThingsRewardScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private IExperienceDistributionScript ExperienceDistributionScript{ get; set; }
        public DarkThingsRewardScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
            ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Inventory.CountOf("Spider's Eye") == 0)
            {
                Subject.Text = "You have no Spider's Eye, which is what I need now.";
            }
            if (source.Inventory.CountOf("Spider's Eye") >= 1)
            {
                int amountToReward = source.Inventory.CountOf("Spider's Eye") * 1000;
                source.TryGiveGold(amountToReward);
                ExperienceDistributionScript.GiveExp(source, amountToReward);
                source.Inventory.RemoveQuantity("Spider's Eye", source.Inventory.CountOf("Spider's Eye"), out _);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive {amountToReward} gold and exp!");
                Subject.Text = "Thank you for grabbing what I needed.";
            }
        }
    }
}
