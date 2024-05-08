using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.EffectScripts.Rogue;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Namotion.Reflection;

namespace Chaos.Scripting.DialogScripts.Quests.LouresCastle;
public class SupplyLouresArmy : DialogScriptBase
{
    private readonly ILogger<SupplyLouresArmy> Logger;
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IEffectFactory EffectFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public SupplyLouresArmy(Dialog subject, ILogger<SupplyLouresArmy> logger, ISimpleCache simpleCache, IItemFactory itemFactory, IEffectFactory effectFactory)
        : base(subject)
    {
        Logger = logger;
        SimpleCache = simpleCache;
        ItemFactory = itemFactory;
        EffectFactory = effectFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }
    public override void OnDisplaying(Aisling source)
    {

        var hasStage = source.Trackers.Enums.TryGetValue(out SupplyLouresStage stage);

        if (!source.Trackers.Enums.HasValue(SickChildStage.SickChildComplete) &&
            !source.Trackers.Enums.HasValue(SickChildStage.SickChildKilled)) return;
        
        if (source.UserStatSheet.Level < 41)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "thibault_initial":
            {
                if (stage == SupplyLouresStage.CompletedAssassin1)
                {
                    Subject.Close(source);
                    source.SendOrangeBarMessage("Thibault won't even speak to you.");
                    return;
                }
                
                var option = new DialogOption
                {
                    DialogKey = "armysupply_initial1",
                    OptionText = "The Sick Child"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }
            
            case "alistair_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "armysupply_initial2",
                    OptionText = "The Sick Child"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }
            
            case "bruce_initial":
            {
                if (source.Trackers.Enums.HasValue(SupplyLouresStage.CompletedAssassin1))
                {
                    Subject.Reply(source, "Get out of my throne room!", "armysupply_kickplayer");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(SupplyLouresStage.CompletedSupply))
                {

                    var option = new DialogOption
                    {
                        DialogKey = "armysupply_kinginitial2",
                        OptionText = "Loures Castle"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                
                if (source.Trackers.Enums.HasValue(SupplyLouresStage.KilledAssassin2))
                {

                    var option = new DialogOption
                    {
                        DialogKey = "armysupply_kinginitial1",
                        OptionText = "Knight Thibault"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "armysupply_initial1":
            {
                if (!hasStage)
                {
                    if (source.Trackers.Enums.HasValue(SickChildStage.SickChildComplete))
                    {
                        Subject.Reply(source, "Skip", "armysupply_startquest10");
                        return;
                    }

                    if (source.Trackers.Enums.HasValue(SickChildStage.SickChildKilled))
                    {
                        Subject.Reply(source, "Skip", "armysupply_startquest1");
                        return;
                    }
                }

                if (hasStage)
                {
                    if (stage == SupplyLouresStage.StartedQuest)
                    {
                        Subject.Reply(source, "Skip", "armysupply_return1");
                        return;
                    }

                    if (stage == SupplyLouresStage.TurnedInSupply)
                    {
                        Subject.Reply(source, "Skip", "armysupply_turnedin1");
                        return;
                    }

                    if (stage == SupplyLouresStage.StartedSupply)
                    {
                        Subject.Reply(source, "Skip", "armysupply_return3");
                        return;
                    }

                    if (stage == SupplyLouresStage.SawAssassin)
                    {
                        Subject.Reply(source, "Skip", "armysupply_sawAssassin1");
                        return;
                    }

                    if (stage == SupplyLouresStage.KilledAssassin)
                    {
                        Subject.Reply(source, "Skip", "armysupply_killedAssassin1");
                        return;
                    }

                    if (stage == SupplyLouresStage.KeptThibaultsSecret)
                    {
                        Subject.Reply(source, "Skip", "armysupply_keptsecret1");
                        return;
                    }

                    if (stage == SupplyLouresStage.CompletedAssassin2)
                    {
                        Subject.Reply(source, "Skip", "armysupply_return6");
                        return;
                    }

                    if (stage == SupplyLouresStage.CompletedSupply)
                    {
                        Subject.Reply(source, "Skip", "armysupply_return5");
                    }
                }

                break;
            }

            case "armysupply_initial2":
            {
                if (!hasStage)
                {
                    Subject.Reply(source,
                        "I am the lead of the Loures Army. My troops can handle their own. With my guidance, there isn't an army in Unora that can handle us.");
                    return;
                }

                if (hasStage)
                {
                    if (stage == SupplyLouresStage.StartedQuest && source.Trackers.Enums.HasValue(SickChildStage.SickChildComplete) && !source.Trackers.Enums.HasValue(SickChildStage.SickChildKilled))
                    {
                        Subject.Reply(source, "Skip", "armysupply_startsupply1");
                        return;
                    }

                    if (stage == SupplyLouresStage.StartedSupply)
                    {
                        Subject.Reply(source, "Skip", "armysupply_return3");
                        return;
                    }

                    if (stage == SupplyLouresStage.SawAssassin)
                    {
                        Subject.Reply(source, "You got what was coming to you, the death of the Princess was tragic.");
                        return;
                    }

                    if (stage == SupplyLouresStage.TurnedInSupply)
                    {
                        Subject.Reply(source,
                            "Skip", "armysupply_return7");
                        return;
                    }

                    if (stage is SupplyLouresStage.CompletedAssassin1 or SupplyLouresStage.CompletedAssassin2 or SupplyLouresStage.KilledAssassin or SupplyLouresStage.KilledAssassin2)
                    {
                        Subject.Reply(source, "You should know, I had nothing to do with that attack.");
                    }

                }
                
                if (stage == SupplyLouresStage.CompletedSupply)
                {
                    Subject.Reply(source,
                        "Thank you again Aisling for bringing those materials back. Go tell Knight Thibault the army is as good as new!");
                    return;
                }

                break;
            }

            case "armysupply_startquest5":
            {
                source.Trackers.Enums.Set(SupplyLouresStage.StartedQuest);
                source.SendOrangeBarMessage("Find Alistair behind the Loures Castle.");
                break;
            }

            case "armysupply_startquest12":
            {
                source.Trackers.Enums.Set(SupplyLouresStage.StartedQuest);
                source.SendOrangeBarMessage("Find Alistair behind the Loures Castle.");
                break;
            }
            
            case "armysupply_startsupply7":
            {
                source.Trackers.Enums.Set(SupplyLouresStage.StartedSupply);
                source.SendOrangeBarMessage("Collect 10 Polished Iron Bars, 5 Pristine Ruby, and 10 Exquisite Cotton.");
                return;
            }

            case "armysupply_return4":
            {
                if (!source.Inventory.HasCount("Polished Iron Bar", 10)
                    || (!source.Inventory.HasCount("Pristine Ruby", 5)
                        || !source.Inventory.HasCount("Exquisite Cotton", 10)))
                {
                    Subject.Reply(source,
                        "The army needs much more than that, we need 10 Polished Iron, 5 Pristine Ruby, and 10 Exquisite Cotton.");
                    return;
                }

                source.Inventory.RemoveQuantity("Polished Iron", 10);
                source.Inventory.RemoveQuantity("Pristine Ruby", 10);
                source.Inventory.RemoveQuantity("Exquisite Cotton", 10);
                source.Trackers.Enums.Set(SupplyLouresStage.TurnedInSupply);
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation("{@AislingName} has received {@ExpAmount} exp and {@GoldAmount}", source.Name, 100000, 20000);

                ExperienceDistributionScript.GiveExp(source, 100000);
                source.TryGiveGold(25000);
                source.TryGiveGamePoints(10);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Supplied the Loures Army",
                        "supplyLoures",
                        MarkIcon.Victory,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                
                source.SendOrangeBarMessage("Go speak to Knight Thibault.");

                break;
            }

            case "armysupply_turnedin3":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.SweetBuns))
                {
                    source.Trackers.Flags.AddFlag(CookingRecipes.SweetBuns);
                    source.SendOrangeBarMessage("Thibault gives you the recipe to make Sweet Buns.");
                }
                else
                {
                    source.TryGiveGamePoints(20);
                    source.TryGiveGold(50000);
                    source.SendOrangeBarMessage("Thibault rewards you 50,000 Gold and 20 Gamepoints.");
                }

                source.Trackers.Enums.Set(SupplyLouresStage.CompletedSupply);
                break;
            }

            case "armysupply_killedassassin5":
            {
                source.Trackers.Enums.Set(SupplyLouresStage.KilledAssassin2);
                source.SendOrangeBarMessage("Speak to King Bruce about Knight Thibault's actions.");
                break;
            }

            case "armysupply_toldtheking6":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("loures_castle");
                var point = new Point(13, 16);
                source.TraverseMap(mapInstance, point);
                source.UserStatSheet.SetHealthPct(5);
                source.UserStatSheet.SetManaPct(5);
                var ardcradheffect = EffectFactory.Create("ardcradh");
                source.Effects.Apply(source, ardcradheffect);
                source.Client.SendAttributes(StatUpdateType.Vitality);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Thrown out of Loures Castle by the Knights",
                        "bannedFromCastle",
                        MarkIcon.Rogue,
                        MarkColor.Orange,
                        1,
                        GameTime.Now));
                source.Trackers.Enums.Set(SupplyLouresStage.CompletedAssassin1);
                source.SendOrangeBarMessage("The Loures Knights beat you up and throw you out.");
                break;
            }

            case "armysupply_keptthibaultssecret2":
            {
                source.Trackers.Enums.Set(SupplyLouresStage.KeptThibaultsSecret);
                return;
            }

            case "armysupply_keptsecret3":
            {
                source.Trackers.Enums.Set(SupplyLouresStage.CompletedAssassin2);
                var item = ItemFactory.Create("silkboots");
                source.GiveItemOrSendToBank(item);
                source.SendOrangeBarMessage("Knight Thibault hands you Silk Boots.");
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation("{@AislingName} has received {@ExpAmount} exp", source.Name, 150000);

                ExperienceDistributionScript.GiveExp(source, 150000);
                source.TryGiveGamePoints(10);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Made amends with Knight Thibault",
                        "thibaultAmends",
                        MarkIcon.Victory,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));
                return;
            }
            case "louresassassin_stabyou":
            {
                var blindeffect = EffectFactory.Create("Blind");
                source.UserStatSheet.SetHealthPct(65);
                source.Effects.Apply(source, blindeffect);
                source.Client.SendAttributes(StatUpdateType.Vitality);
                return;
            }

            case "armysupply_kickplayer":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("loures_castle");
                var point = new Point(13, 16);
                source.TraverseMap(mapInstance, point);
                source.UserStatSheet.SetHealthPct(5);
                source.UserStatSheet.SetManaPct(5);
                var ardcradheffect = EffectFactory.Create("ardcradh");
                source.Effects.Apply(source, ardcradheffect);
                source.Client.SendAttributes(StatUpdateType.Vitality);
                return;
            }
        }
    }
    }