using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Common.Utilities;
using Chaos.Formulae.LevelUp;
using Chaos.Common.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Formulae;
using Chaos.Storage.Abstractions;
using Chaos.Containers;

namespace Chaos.Scripts.DialogScripts.Mileth
{
    public class TerrorChestScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISimpleCache SimpleCache;
        public TerrorChestScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache) : base(subject)
        {
            ItemFactory = itemFactory;
            SimpleCache = simpleCache;
        }

        public override void OnDisplaying(Aisling source)
        {
            //Check which item they chose in the previous dialog option.
            if (Subject.Template.TemplateKey.EqualsI("terrorChest_randomArmorDye"))
            {
                //Generate a random town
                var random = new Random();
                var armorDye = new List<string> { "Mileth", "Rucesion", "Suomi", "Loures" };
                int index = random.Next(armorDye.Count);
                //Start creating the item
                var item = ItemFactory.Create("armorDyeContainer");
                //Give it a name and color based on the town
                item.DisplayName = $"{armorDye[index]} Armor Dye";
                if (armorDye[index].EqualsI("Mileth"))
                    item.Color = DisplayColor.Green;
                if (armorDye[index].EqualsI("Rucesion"))
                    item.Color = DisplayColor.Blue;
                if (armorDye[index].EqualsI("Suomi"))
                    item.Color = DisplayColor.Red;
                if (armorDye[index].EqualsI("Loures"))
                    item.Color = DisplayColor.White;

                //Give 20% of current TNL
                int TNL = LevelUpFormulae.Default.CalculateTnl(source);
                int twentyPercent = Convert.ToInt32(0.20 * TNL);

                source.GiveExp(twentyPercent);
                //Give Gold
                source.TryGiveGold(50000);

                //Give item to inventory we built previously
                source.TryGiveItem(item);
                //Lets send them an orange bar message and give them a Legend Mark for completing the quest
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received {item.DisplayName} and 50,000 coins!");
                source.Legend.AddOrAccumulate(new LegendMark("Vanquished Terror of the Crypt", "cryptTerror", MarkIcon.Victory, MarkColor.White, 1, Time.GameTime.Now));

                //Warp Player back to Tavern
                MapInstance mapInstance;
                mapInstance = SimpleCache.Get<MapInstance>("mileth_tavern");
                source.TraverseMap(mapInstance, new Point(9, 10));
                //Close Dialog
                Subject.Close(source);
            }

            if (Subject.Template.TemplateKey.EqualsI("terrorChest_randomOvercoat"))
            {
                //Lets generate a random overcoat to give to the player
                var templateKeyRewards = new List<string> { "dyeableTrainingOutfit" };
                var random = new Random();
                int index = random.Next(templateKeyRewards.Count);
                //Create the item
                var item = ItemFactory.Create(templateKeyRewards[index]);

                //Give 20% of current TNL
                int TNL = LevelUpFormulae.Default.CalculateTnl(source);
                int twentyPercent = Convert.ToInt32(0.20 * TNL);

                source.GiveExp(twentyPercent);
                //Give Gold
                source.TryGiveGold(50000);

                //Give item to inventory we built previously
                source.TryGiveItem(item);
                //Lets send them an orange bar message and give them a Legend Mark for completing the quest
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received {item.DisplayName} and 50,000 coins!");
                source.Legend.AddOrAccumulate(new LegendMark("Vanquished Terror of the Crypt", "cryptTerror", MarkIcon.Victory, MarkColor.White, 1, Time.GameTime.Now));
                source.TimedEvents.AddEvent(Data.TimedEvent.TimedEventId.TerrorOfTheCrypt, TimeSpan.FromDays(1));

                //Warp Player back to Tavern
                MapInstance mapInstance;
                mapInstance = SimpleCache.Get<MapInstance>("mileth_tavern");
                source.TraverseMap(mapInstance, new Point(9, 10));
                //Close Dialog
                Subject.Close(source);
            }
        }
    }
}
