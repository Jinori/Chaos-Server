using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Mileth;

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
        var isArmorDye = Subject.Template.TemplateKey.EqualsI("terrorChest_randomArmorDye");
        var isOvercoat = Subject.Template.TemplateKey.EqualsI("terrorChest_randomOvercoat");

        if (isArmorDye || isOvercoat)
        {
            Item item;

            if (isArmorDye)
            {
                var armorDye = new List<string> { "Mileth", "Rucesion", "Suomi", "Loures" };
                var random = new Random();
                var index = random.Next(armorDye.Count);
                item = ItemFactory.Create("armorDyeContainer");
                item.DisplayName = $"{armorDye[index]} Armor Dye";

                item.Color = armorDye[index] switch
                {
                    "Mileth"   => DisplayColor.Green,
                    "Rucesion" => DisplayColor.Blue,
                    "Suomi"    => DisplayColor.Red,
                    "Loures"   => DisplayColor.White,
                    _          => item.Color
                };
            }
            else // isOvercoat
            {
                var templateKeyRewards = new List<string> { "dyeableTrainingOutfit" };
                var index = new Random().Next(templateKeyRewards.Count);
                item = ItemFactory.Create(templateKeyRewards[index]);
            }

            var tnl = LevelUpFormulae.Default.CalculateTnl(source);
            var expAmount = Convert.ToInt32(0.30 * tnl);
            ExperienceDistributionScript.GiveExp(source, expAmount);
            source.TryGiveGold(20000);
            source.TryGiveItem(ref item);
            source.TryGiveGamePoints(10);

            source.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You've received {item.DisplayName}, 20,000 coins and 10 game points!");

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Vanquished Terror of the Crypt",
                    "cryptTerror",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            source.Trackers.TimedEvents.AddEvent("TerrorOfTheCrypt", TimeSpan.FromDays(1));

            var mapInstance = SimpleCache.Get<MapInstance>("mileth_tavern");
            source.TraverseMap(mapInstance, new Point(9, 10));
            Subject.Close(source);
        }
    }
}