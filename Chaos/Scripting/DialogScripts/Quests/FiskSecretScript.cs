using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests;

public class FiskSecretScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public FiskSecretScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out FiskSecretStage stage);
        var hasStage2 = source.Trackers.Enums.TryGetValue(out FiskRemakeBouquet stage2);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region Fisk
            
            case "fisk_initial":
            {
                if (hasStage && (stage == FiskSecretStage.CompletedFiskSecret))
                    return;
                
                if (source.UserStatSheet.Level >= 11)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "FiskSecret_initial",
                        OptionText = "Fisk's Secret"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }
                return;
            }
            
            case "fisksecret_initial":
            {
                if (hasStage && (stage == FiskSecretStage.ExposedFisk))
                {
                    Subject.Reply(source, "You really told them? I knew I couldn't trust you Aisling! Begone! We are through. I can't show my face anywhere. *Fisk is embarrassed and walks off*");

                    return;
                }
                if ((hasStage && (stage == FiskSecretStage.Started)) || (stage == FiskSecretStage.StartedWaterLilies) || (stage == FiskSecretStage.CollectedWaterLilies))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_startedreturn");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.Started2) || (stage == FiskSecretStage.StartedPetunias) || (stage == FiskSecretStage.CollectedPetunias))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_startedreturn2");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.Started3) || (stage == FiskSecretStage.StartedPinkRose) || (stage == FiskSecretStage.CollectedPinkRose))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_startedreturn3");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.Started4) || (stage == FiskSecretStage.StartedBouquet) || (stage == FiskSecretStage.CollectedBouquet))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_startedreturn4");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.Started5) || (stage == FiskSecretStage.DeliverBouquet))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_startedreturn5");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.DeliveredBouquet))
                {
                    Subject.Reply(source, "Skip", "deliveredbouquet");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.AngeredThulin))
                {
                    Subject.Reply(source, "Skip", "fiskAngeredThulin");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.AngeredThulinReturn))
                {
                    Subject.Reply(source, "Skip", "fiskAngeredThulinCheckWaterlily");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.WontTell))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_wonttell");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.DeliveredBouquet))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_deliveredbouquet");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.Started6))
                {
                    Subject.Reply(source, "Skip", "FiskSecret_collectedbrandy");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.BrandyTurnin))
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("fiskbrandy", out var timedEvent))
                    {
                        Subject.Reply(source, $"I haven't been over to see her yet... Come see me again in {timedEvent.Remaining.ToReadableString()}");

                        return;
                    }
                    Subject.Reply(source, "Skip", "FiskSecret_fiskcheckin");
                }


                break;
            }
            case "petunia_start3":
            {
                source.SendOrangeBarMessage("Go talk to Eeva at the Inn to get Petunias.");
                source.Trackers.Enums.Set(FiskSecretStage.Started2);

                return;
            }

            case "fisksecret_start5":
            {
                source.SendOrangeBarMessage("Go talk to Thulin at the theater to get water lilies.");
                source.Trackers.Enums.Set(FiskSecretStage.Started);
                return;
            }
            case "pinkrose_start4":
            {
                source.SendOrangeBarMessage("Go talk to Vivianne at Deoch's Shrine to get a Pink Rose.");
                source.Trackers.Enums.Set(FiskSecretStage.Started3);
                return;
            }
            case "bouquet_start4":
            {
                source.Trackers.Enums.Set(FiskSecretStage.Started4);
                return;
            }
            case "bouquet_start9":
            {
                source.Trackers.Enums.Set(FiskSecretStage.Started4);
                source.TryGiveGold(25000);
                source.SendOrangeBarMessage("Fisk hands you 25,000 gold. Ask Viveka to make the bouquet.");
                return;
            }
            case "petunia_turnin":
            {
                if (!source.Inventory.RemoveQuantity("grapes", 20))
                {
                    Subject.Reply(source, "You don't have enough grapes, where are they?");
                    return;
                }

                source.Trackers.Enums.Set(FiskSecretStage.CollectedPetunias);

                var petunia = ItemFactory.Create("petunia");
                petunia.Count = 5;
                source.TryGiveItems(petunia);

                Subject.Reply(
                    source,
                    "Thank you Aisling, now my guest will have grapes. Good luck with whatever you're doing with those Petunias.");

                return;

            }
            case "fisksecret_collectedwaterlilies":
            {
                if (!source.Inventory.HasCount("waterlily", 5))
                {
                    Subject.Reply(source, "I thought you got the waterlilies? Where did they go?");
                    source.SendOrangeBarMessage("You don't have enough Water Lilies.");
                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.Started2);
                break;

            }
            case "fisksecret_collectedpetunias":
            {
                if (!source.Inventory.HasCount("petunia", 5))
                {
                    Subject.Reply(source, "Where are the petunias? Did you lose them?");
                    source.SendOrangeBarMessage("You don't have enough Petunias.");
                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.Started3);
                break;

            }
            case "fisksecret_collectedpinkrose":
            {
                if (!source.Inventory.HasCount("pinkrose", 1))
                {
                    Subject.Reply(source, "The Pink rose? Where is it?");
                    source.SendOrangeBarMessage("You don't have the Pink Rose.");
                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.Started4);
                break;

            }
            case "fisksecret_collectedbouquet":
            {
                if (!source.Inventory.HasCount("specialbouquet", 1) && !source.Equipment.Contains("specialBouquet"))
                {
                    Subject.Reply(source, "Where's the bouquet? I was so excited!");
                    source.SendOrangeBarMessage("You don't have the Bouquet.");
                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.Started5);
                break;

            }
            case "fisksecret_collectedbrandy":
            {
                if (!source.Inventory.RemoveQuantity("brandy", 10))
                {
                    Subject.Reply(source, "Where's the Brandy?");
                    source.SendOrangeBarMessage("You don't have enough Brandy.");
                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.BrandyTurnin);
                source.Trackers.TimedEvents.AddEvent("fiskbrandy", TimeSpan.FromHours(1), true);
                break;

            }
            case "fisksecret_fiskcheckin2":
            {
                source.Trackers.Enums.Set(FiskSecretStage.CompletedFiskSecret);
                source.TryGiveGold(125000);
                source.TryGiveGamePoints(10);
                ExperienceDistributionScript.GiveExp(source, 300000);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Inspired Fisk to reach for the stars.",
                        "FiskSecret",
                        MarkIcon.Heart,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));

                break;

            }
            case "givebertil_start":
            {
                if (!source.Inventory.HasCount("specialbouquet", 1) && (!source.Equipment.Contains("specialbouquet")))
                {
                    Subject.Reply(source, "Where'd you put the Bouquet?");
                    source.SendOrangeBarMessage("You don't have the Bouquet.");
                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.DeliverBouquet);
                break;

            }
            case "deliveredbouquet4":
            {
                source.Trackers.Enums.Set(FiskSecretStage.CompletedFiskSecret);
                source.TryGiveGold(75000);
                source.TryGiveGamePoints(10);
                ExperienceDistributionScript.GiveExp(source, 250000);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Kept Fisk's Secret",
                        "FiskSecret",
                        MarkIcon.Heart,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                break;

            }
            case "deliveredbouquet11":
            {
                source.Trackers.Enums.Set(FiskSecretStage.Started6);
                source.SendOrangeBarMessage("Bring Fisk 10 Brandy");
                break;

            }

            #endregion

            #region ThulinWaterLily
            
            case "fiskangeredthulin3":
            {
                source.Trackers.Enums.Set(FiskSecretStage.AngeredThulinReturn);

                return;
            }

            case "thulin_initial":
            {
                if ((hasStage && (stage == FiskSecretStage.Started)) || (stage == FiskSecretStage.AngeredThulin) || (stage == FiskSecretStage.StartedWaterLilies) || (stage == FiskSecretStage.CollectedWaterLilies))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "WaterLily_initial",
                        OptionText = "Water Lilies"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "waterlily_initial":
            {
                if (hasStage && (stage == FiskSecretStage.AngeredThulin))
                {
                    Subject.Reply(source, "Skip", "angeredthulin");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.StartedWaterLilies))
                {
                    Subject.Reply(source, "Skip", "waterlily_return");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.CollectedWaterLilies))
                {
                    Subject.Reply(source, "Skip", "Waterlily_thanks");
                }

                break;
            }

            case "waterlily_start3":
            {
                source.SendOrangeBarMessage("Go buy 10 Wine from Viveka and bring them back.");
                source.Trackers.Enums.Set(FiskSecretStage.StartedWaterLilies);

                return;
            }

            case "waterlily_end":
            {
                source.SendOrangeBarMessage("Thulin walks away from you.");
                source.Trackers.Enums.Set(FiskSecretStage.AngeredThulin);

                return;
            }

            case "waterlily_turnin":
            {
                if (!source.Inventory.RemoveQuantity("wine", 10))
                {
                    Subject.Reply(source, "Where is my wine? Please go get it.");

                    return;
                }

                source.Trackers.Enums.Set(FiskSecretStage.CollectedWaterLilies);

                var waterlily = ItemFactory.Create("waterlily");
                waterlily.Count = 5;
                source.TryGiveItems(waterlily);
                Subject.Reply(source, "I knew I can count on you. I wish I knew the secret, but this will do. Thank you and Good luck!");

                return;

            }
            #endregion
            
            #region EevaPetunia
            case "eeva_initial":
            {
                if (hasStage && (stage == FiskSecretStage.Started2) || (stage == FiskSecretStage.CollectedPetunias) || (stage == FiskSecretStage.StartedPetunias))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "Petunia_initial",
                        OptionText = "Petunias"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;
            }
            case "petunia_initial":
            {
                if (hasStage && (stage == FiskSecretStage.Started2))
                {
                    Subject.Reply(source, "Skip", "petunia_start4");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.CollectedPetunias))
                {
                    Subject.Reply(source, "Skip", "petunia_thanks");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.StartedPetunias))
                {
                    Subject.Reply(source, "Skip", "petunia_return");
                }

                break;
            }

            case "petunia_start8":
            {
                source.Trackers.Enums.Set(FiskSecretStage.StartedPetunias);
                source.SendOrangeBarMessage("Bring Eeva 20 Grapes.");

                return;
            }
            
            #endregion

            #region ViviannePinkRose
            case "vivianne_initial":
            {
                if (hasStage && (stage == FiskSecretStage.Started3) || (stage == FiskSecretStage.CollectedPinkRose) || (stage == FiskSecretStage.MushroomStart) || (stage == FiskSecretStage.StartedPinkRose) || (stage == FiskSecretStage.WontTell) || (stage == FiskSecretStage.WontTell2))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "pinkrose_initial",
                        OptionText = "Pink Rose"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;
            }
            case "pinkrose_initial":
            {
                if (hasStage && (stage == FiskSecretStage.Started3))
                {
                    Subject.Reply(source, "Skip", "pinkrose_start5");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.CollectedPinkRose))
                {
                    Subject.Reply(source, "Skip", "pinkrose_thanks");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.StartedPinkRose))
                {
                    Subject.Reply(source, "Skip", "pinkrose_return");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.WontTell))
                {
                    Subject.Reply(source, "Until you're a bit more honest, please go away. You aren't getting my pink roses.");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.WontTell2))
                {
                    Subject.Reply(source, "Skip", "pinkrose_start20");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.MushroomStart))
                {
                    Subject.Reply(source, "Skip", "MushroomReturn");
                }

                break;
            }
            case "pinkrose_end":
            {
                source.Trackers.Enums.Set(FiskSecretStage.WontTell);
                Subject.Reply(source, "Skip", "Close");

                break;
            }
            case "fisksecret_wonttell2":
            {
                source.Trackers.Enums.Set(FiskSecretStage.WontTell2);

                return;
            }
            case "pinkrose_start13":
            {
                if (!source.TryTakeGold(20000))
                {
                    source.SendOrangeBarMessage("You don't have enough gold to donate.");

                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.CollectedPinkRose);

                var pinkrose = ItemFactory.Create("pinkrose");
                pinkrose.Count = 1;
                source.TryGiveItems(pinkrose);

                Subject.Reply(
                    source,
                    "That will do. Here's your pink rose. Thank you, tell Fisk I said hi.");

                return;

            }
            case "pinkrose_start16":
            {
                source.Trackers.Enums.Set(FiskSecretStage.CollectedPinkRose);

                var pinkrose = ItemFactory.Create("pinkrose");
                pinkrose.Count = 1;
                source.TryGiveItems(pinkrose);

                Subject.Reply(
                    source,
                    "Close");

                return;

            }
            case "pinkrose_start24":
            {
                if (!source.TryTakeGold(10000))
                {
                    source.SendOrangeBarMessage("You don't have enough gold to donate.");

                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.CollectedPinkRose);

                var pinkrose = ItemFactory.Create("pinkrose");
                pinkrose.Count = 1;
                source.TryGiveItems(pinkrose);

                Subject.Reply(
                    source,
                    "That will do. Here's your pink rose. Thank you, tell Fisk I said hi.");

                return;

            }
            case "pinkrose_start26":
            {
                source.Trackers.Enums.Set(FiskSecretStage.MushroomStart);

                return;
            }
            case "mushroomreturn2":
            {
                if (!source.Inventory.RemoveQuantity("mushroom", 10))
                {
                    Subject.Reply(source, "Where are my mushrooms?");
                    source.SendOrangeBarMessage("You do not have enough mushrooms.");

                    return;
                }
                source.Trackers.Enums.Set(FiskSecretStage.CollectedPinkRose);

                var pinkrose = ItemFactory.Create("pinkrose");
                pinkrose.Count = 1;
                source.TryGiveItems(pinkrose);

                Subject.Reply(
                    source,
                    "That will do. Here's your pink rose. Thank you, tell Fisk I said hi.");

                break;
            }
            #endregion

            #region VivekaBouquet

            case "viveka_initial":
            {

                if (hasStage)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "bouquet_initial",
                        OptionText = "Bouquet"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }
                break;
            }
            case "bouquet_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("craftbouquet", out var timedEvent))
                {
                    Subject.Reply(source, $"I am not quite done yet. It can wait till later. Come back in {timedEvent.Remaining.ToReadableString()}");

                    return;
                }
                if (source.Trackers.TimedEvents.HasActiveEvent("craftbouquet2", out var timedEvent1))
                {
                    Subject.Reply(source, $"I am not quite done yet. I will work on it soon. Come back in {timedEvent1.Remaining.ToReadableString()}");

                    return;
                }
                if (source.Trackers.TimedEvents.HasActiveEvent("craftbouquet3", out var timedEvent2))
                {
                    Subject.Reply(source, $"I am not quite done yet. I am working on it right now. Come back in {timedEvent2.Remaining.ToReadableString()}");

                    return;
                }
                if (source.Trackers.TimedEvents.HasActiveEvent("craftbouquet4", out var timedEvent3))
                {
                    Subject.Reply(source, $"I am not quite done yet. I am a bit busy now. Come back in {timedEvent3.Remaining.ToReadableString()}");

                    return;
                }
                if (hasStage && (stage == FiskSecretStage.Started4))
                {
                    Subject.Reply(source, "Skip", "bouquet_start10");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.StartedBouquet))
                {
                    Subject.Reply(source, "Skip", "bouquet_return");

                    return;
                }
                if (hasStage2 && (stage2 == FiskRemakeBouquet.BouquetWait))
                {
                    Subject.Reply(source, "Skip", "bouquet_remake");

                    return;
                }
                {
                    if (source.Inventory.HasCount("specialbouquet", 1) || (source.Equipment.Contains("specialBouquet")))
                    {
                        Subject.Reply(source, "I already made you a bouquet.");

                        return;
                    }
                    Subject.Reply(source, "Skip", "RemakeBouquet");
                }

                break;
            }
            case "bouquet_start15":
            {
                if (!source.Inventory.HasCount("petunia", 5)
                    && (!source.Inventory.HasCount("pinkrose", 1) && (!source.Inventory.HasCount("waterlily", 5))))
                {
                    Subject.Reply(source, "I think you're missing some flowers. I need the five water lilies, five petunias, and one pink rose.");
                    source.SendOrangeBarMessage("Looks like you're missing some flowers.");
                    return;
                }
                if (!source.TryTakeGold(25000))
                {
                    Subject.Reply(source, "You can't afford my services. Come back when you have enough gold.");
                    source.SendOrangeBarMessage("You don't have enough gold to pay Viveka.");

                    return;
                }

                source.Inventory.RemoveQuantity("petunia", 5);
                source.Inventory.RemoveQuantity("waterlily", 5);
                source.Inventory.RemoveQuantity("pinkrose", 1);
                source.Trackers.Enums.Set(FiskSecretStage.StartedBouquet);
                source.Trackers.TimedEvents.AddEvent("craftbouquet", TimeSpan.FromHours(1), true);

                return;
            }
            case "bouquet_start19":
            {
                if (!source.Inventory.HasCount("petunia", 5)
                    && (!source.Inventory.HasCount("pinkrose", 1) && (!source.Inventory.HasCount("waterlily", 5))))
                {
                    Subject.Reply(source, "I think you're missing some flowers. I need the five water lilies, five petunias, and one pink rose.");
                    source.SendOrangeBarMessage("Looks like you're missing some flowers.");
                    return;
                }
                if (!source.TryTakeGold(15000))
                {
                    Subject.Reply(source, "You can't afford my services. Come back when you have enough gold.");
                    source.SendOrangeBarMessage("You don't have enough gold to pay Viveka.");

                    return;
                }

                source.Inventory.RemoveQuantity("petunia", 5);
                source.Inventory.RemoveQuantity("waterlily", 5);
                source.Inventory.RemoveQuantity("pinkrose", 1);
                source.Trackers.Enums.Set(FiskSecretStage.StartedBouquet);
                source.Trackers.TimedEvents.AddEvent("craftbouquet2", TimeSpan.FromMinutes(30), true);

                return;
            }
            case "bouquet_start22":
            {
                if (!source.Inventory.HasCount("petunia", 5)
                    && (!source.Inventory.HasCount("pinkrose", 1) && (!source.Inventory.HasCount("waterlily", 5))))
                {
                    Subject.Reply(source, "I think you're missing some flowers. I need the five water lilies, five petunias, and one pink rose.");
                    source.SendOrangeBarMessage("Looks like you're missing some flowers.");
                    return;
                }
                if (!source.TryTakeGold(10000))
                {
                    Subject.Reply(source, "You can't afford my services. Come back when you have enough gold.");
                    source.SendOrangeBarMessage("You don't have enough gold to pay Viveka.");

                    return;
                }

                source.Inventory.RemoveQuantity("petunia", 5);
                source.Inventory.RemoveQuantity("waterlily", 5);
                source.Inventory.RemoveQuantity("pinkrose", 1);
                source.Trackers.Enums.Set(FiskSecretStage.StartedBouquet);
                source.Trackers.TimedEvents.AddEvent("craftbouquet3", TimeSpan.FromMinutes(5), true);

                return;
            }
            case "remakebouquet3":
            {
                if (!source.Inventory.HasCount("petunia", 5)
                    && (!source.Inventory.HasCount("pinkrose", 1) && (!source.Inventory.HasCount("waterlily", 5))))
                {
                    Subject.Reply(source, "I think you're missing some flowers. I need the five water lilies, five petunias, and one pink rose.");
                    source.SendOrangeBarMessage("Looks like you're missing some flowers.");
                    return;
                }
                if (!source.TryTakeGold(50000))
                {
                    Subject.Reply(source, "You can't afford my services. Come back when you have enough gold.");
                    source.SendOrangeBarMessage("You don't have enough gold to pay Viveka.");

                    return;
                }

                source.Inventory.RemoveQuantity("petunia", 5);
                source.Inventory.RemoveQuantity("waterlily", 5);
                source.Inventory.RemoveQuantity("pinkrose", 1);
                source.Trackers.Enums.Set(FiskRemakeBouquet.BouquetWait);
                source.Trackers.TimedEvents.AddEvent("craftbouquet4", TimeSpan.FromHours(1), true);

                return;
            }
            case "bouquet_return2":
            {
                source.Trackers.Enums.Set(FiskSecretStage.CollectedBouquet);

                var bouquet = ItemFactory.Create("specialbouquet");
                bouquet.Count = 1;
                source.TryGiveItems(bouquet);

                return;
            }
            case "bouquet_remake2":
            {
                source.Trackers.Enums.Remove<FiskRemakeBouquet>();

                var bouquet = ItemFactory.Create("specialbouquet");
                bouquet.Count = 1;
                source.TryGiveItems(bouquet);

                return;
            }
            #endregion

            #region ExposeFisk
            case "exposefisk_thulin":
            {
                var waterlily = ItemFactory.Create("waterlily");
                waterlily.Count = 10;
                source.TryGiveItems(waterlily);
                source.Trackers.Enums.Set(FiskSecretStage.ExposedFisk);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Betrayed Fisk's Trust",
                        "BetrayedFisk",
                        MarkIcon.Warrior,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                return;
            }
            case "exposefisk_eeva":
            {
                source.Trackers.Enums.Set(FiskSecretStage.ExposedFisk);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Betrayed Fisk's Trust",
                        "BetrayedFisk",
                        MarkIcon.Warrior,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                return;
            }
            case "exposefisk_vivianne":
            {
                var pinkrose = ItemFactory.Create("pinkrose");
                pinkrose.Count = 5;
                source.TryGiveItems(pinkrose);
                source.Trackers.Enums.Set(FiskSecretStage.ExposedFisk);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Betrayed Fisk's Trust",
                        "BetrayedFisk",
                        MarkIcon.Warrior,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                return;
            }
            case "exposefisk_viveka":
            {
                source.Trackers.Enums.Set(FiskSecretStage.ExposedFisk);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Betrayed Fisk's Trust",
                        "BetrayedFisk",
                        MarkIcon.Warrior,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                return;
            }
            case "exposefisk_bertil":
            {
                source.Trackers.Enums.Set(FiskSecretStage.ExposedFisk);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Betrayed Fisk's Trust",
                        "BetrayedFisk",
                        MarkIcon.Warrior,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                return;
            }
            #endregion

            #region BertilBouquet
            case "bertil_initial":
            {
                if (hasStage && (stage == FiskSecretStage.DeliverBouquet) || (stage == FiskSecretStage.DeliveredBouquet))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "deliverbouquet_initial",
                        OptionText = "Special Delivery"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;
            }
            case "deliverbouquet_initial":
            {
                if (hasStage && (stage == FiskSecretStage.DeliverBouquet))
                {
                    Subject.Reply(source, "Skip", "deliverbouquet");

                    return;
                }

                if (hasStage && (stage == FiskSecretStage.DeliveredBouquet))
                {
                    Subject.Reply(source, "Thank you again Aisling for bringing me the bouquet, they sure are lovely. I hope my secret admirer comes for me soon.");
                }

                break;
            }
            case "deliverbouquet2":
            {
                if (!source.Inventory.HasCount("specialbouquet", 1) && (!source.Equipment.Contains("specialBouquet")))
                {
                    Subject.Reply(source, "What bouquet? You don't have one.");
                    source.SendOrangeBarMessage("You don't have the Bouquet.");

                    return;
                }

                source.Inventory.TryGetRemove("specialbouquet", out _);
                source.Equipment.Remove("specialbouquet");
                source.Trackers.Enums.Set(FiskSecretStage.DeliveredBouquet);
                break;

            }
            #endregion
        }
    }
}