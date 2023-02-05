using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Mileth;

public class TerrorChestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public TerrorChestScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        //Check which item they chose in the previous dialog option.
        if (Subject.Template.TemplateKey.EqualsI("terrorChest_randomArmorDye"))
        {
            //Generate a random town
            var random = new Random();
            var armorDye = new List<string> { "Mileth", "Rucesion", "Suomi", "Loures" };
            var index = random.Next(armorDye.Count);
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
            var tnl = LevelUpFormulae.Default.CalculateTnl(source);
            var twentyPercent = Convert.ToInt32(0.20 * tnl);

            ExperienceDistributionScript.GiveExp(source, twentyPercent);
            //Give Gold
            source.TryGiveGold(50000);

            //Give item to inventory we built previously
            source.TryGiveItem(item);
            source.TryGiveGamePoints(15);
            //Lets send them an orange bar message and give them a Legend Mark for completing the quest
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received {item.DisplayName}, 50,000 coins and 5 game points!");

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Vanquished Terror of the Crypt",
                    "cryptTerror",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            //Warp Player back to Tavern
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_tavern");
            source.TraverseMap(mapInstance, new Point(9, 10));
            //Close Dialog
            Subject.Close(source);
        }

        if (Subject.Template.TemplateKey.EqualsI("terrorChest_randomOvercoat"))
        {
            //Lets generate a random overcoat to give to the player
            var templateKeyRewards = new List<string> { "dyeableTrainingOutfit" };
            var random = new Random();
            var index = random.Next(templateKeyRewards.Count);
            //Create the item
            var item = ItemFactory.Create(templateKeyRewards[index]);

            //Give 20% of current TNL
            var tnl = LevelUpFormulae.Default.CalculateTnl(source);
            var twentyPercent = Convert.ToInt32(0.20 * tnl);

            ExperienceDistributionScript.GiveExp(source, twentyPercent);
            //Give Gold
            source.TryGiveGold(50000);

            //Give item to inventory we built previously
            source.TryGiveItem(item);
            source.TryGiveGamePoints(15);
            //Lets send them an orange bar message and give them a Legend Mark for completing the quest
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received {item.DisplayName}, 50,000 coins and 5 game points!");

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Vanquished Terror of the Crypt",
                    "cryptTerror",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            source.TimedEvents.AddEvent(TimedEvent.TimedEventId.TerrorOfTheCrypt, TimeSpan.FromDays(1));

            //Warp Player back to Tavern
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_tavern");
            source.TraverseMap(mapInstance, new Point(9, 10));
            //Close Dialog
            Subject.Close(source);
        }
    }
}