﻿using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class TerrorChestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<TerrorChestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public TerrorChestScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        ILogger<TerrorChestScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        if (!source.Trackers.TimedEvents.HasActiveEvent("TerrorOfTheCrypt", out _))
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
                        "Suomi"    => DisplayColor.Apple,
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

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Gold,
                          Topics.Entities.Experience,
                          Topics.Entities.Dialog,
                          Topics.Entities.Quest,
                          Topics.Entities.Item)
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation(
                          "{@AislingName} has received {@GoldAmount} gold, {@ExpAmount} exp and {@ItemName} from a quest",
                          source.Name,
                          20000,
                          expAmount,
                          item.DisplayName);

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

                source.Trackers.TimedEvents.AddEvent("TerrorOfTheCrypt", TimeSpan.FromDays(1), true);
            }
        }
        else
            source.SendActiveMessage("You have already received today's rewards for this quest.");

        var mapInstance = SimpleCache.Get<MapInstance>("mileth_tavern");
        source.TraverseMap(mapInstance, new Point(9, 10));
        Subject.Close(source);
    }
}