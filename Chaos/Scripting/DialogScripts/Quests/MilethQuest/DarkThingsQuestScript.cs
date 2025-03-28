using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests.MilethQuest;

public class DarkThingsQuestScript : DialogScriptBase
{
    private readonly ILogger<DarkThingsQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public DarkThingsQuestScript(Dialog subject, ILogger<DarkThingsQuestScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out DarkThingsStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenpercent = Convert.ToInt32(.10 * tnl);

        if (tenpercent > 320000)
        {
            tenpercent = 320000;
        }

        var fifteenpercent = Convert.ToInt32(0.15 * tnl);

        if (fifteenpercent > 480000)
        {
            fifteenpercent = 480000;
        }
        var twentypercent = Convert.ToInt32(0.20 * tnl);

        if (twentypercent > 640000)
        {
            twentypercent = 640000;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "dar_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "DarkThings_initial",
                    OptionText = "Dark Things"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }

                break;

            case "darkthings_initial":
                if (!hasStage || (stage == DarkThingsStage.None))
                    if (source.Trackers.TimedEvents.HasActiveEvent("DarkThingsCd", out var timedEvent))
                    {
                        Subject.Reply(source, $"I have enough for now. Return later. (({timedEvent.Remaining.ToReadableString()}))");

                        return;
                    }

                if (stage == DarkThingsStage.StartedSpidersEye)
                {
                    Subject.Reply(source, "skip", "darkthings_startse");

                    return;
                }

                if (stage == DarkThingsStage.StartedCentipedesGland)
                {
                    Subject.Reply(source, "skip", "darkthings_startcg");

                    return;
                }

                if (stage == DarkThingsStage.StartedSpidersSilk)
                {
                    Subject.Reply(source, "skip", "darkthings_startss");

                    return;
                }

                if (stage == DarkThingsStage.StartedBatsWing)
                {
                    Subject.Reply(source, "skip", "darkthings_startbw");

                    return;
                }

                if (stage == DarkThingsStage.StartedGiantBatsWing)
                {
                    Subject.Reply(source, "skip", "darkthings_startgbw");

                    return;
                }

                if (stage == DarkThingsStage.StartedWhiteBatsWing)
                {
                    Subject.Reply(source, "skip", "darkthings_startwbw");

                    return;
                }

                if (stage == DarkThingsStage.StartedScorpionSting)
                    Subject.Reply(source, "skip", "darkthings_startscs");
                
                if (stage == DarkThingsStage.StartedMarauderSpine)
                    Subject.Reply(source, "skip", "darkthings_startms");
                
                if (stage == DarkThingsStage.StartedKardiFur)
                    Subject.Reply(source, "skip", "darkthings_startkf");
                
                if (stage == DarkThingsStage.StartedMimicTeeth)
                    Subject.Reply(source, "skip", "darkthings_startmt");
                
                if (stage == DarkThingsStage.StartedSuccubusHair)
                    Subject.Reply(source, "skip", "darkthings_startsh");

                break;

            case "darkthings_yes":
                if (!hasStage || (stage == DarkThingsStage.None))
                {
                    var randomDarkThingsStage = DarkThingsStage.None;

                    if (source.UserStatSheet.Level < 11)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedSpidersEye, DarkThingsStage.StartedCentipedesGland, DarkThingsStage.StartedSpidersSilk
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomDarkThingsStage);
                    }

                    if (source.UserStatSheet.Level is >= 11 and < 21)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedSpidersEye, DarkThingsStage.StartedCentipedesGland, DarkThingsStage.StartedSpidersSilk,
                            DarkThingsStage.StartedBatsWing
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomDarkThingsStage);
                    }

                    if (source.UserStatSheet.Level is >= 21 and < 31)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedBatsWing, DarkThingsStage.StartedCentipedesGland, DarkThingsStage.StartedScorpionSting
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomDarkThingsStage);
                    }

                    if (source.UserStatSheet.Level is >= 31 and < 51)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedGiantBatsWing, DarkThingsStage.StartedScorpionSting, DarkThingsStage.StartedWhiteBatsWing
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomDarkThingsStage);
                    }
                    
                    if (source.UserStatSheet.Level >= 51)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedMarauderSpine, DarkThingsStage.StartedKardiFur, DarkThingsStage.StartedMimicTeeth, DarkThingsStage.StartedSuccubusHair
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomDarkThingsStage);
                    }

                    switch (randomDarkThingsStage)
                    {
                        case DarkThingsStage.StartedSpidersEye:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Spider's Eye.");
                        }

                            break;

                        case DarkThingsStage.StartedSpidersSilk:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Spider's Silk.");
                        }

                            break;

                        case DarkThingsStage.StartedCentipedesGland:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Centipede's Gland.");
                        }

                            break;
                        case DarkThingsStage.StartedBatsWing:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Bat's Wing.");
                        }

                            break;
                        case DarkThingsStage.StartedScorpionSting:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Scorpion's Sting.");
                        }

                            break;

                        case DarkThingsStage.StartedGiantBatsWing:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Giant Bat's Wing.");
                        }

                            break;
                        case DarkThingsStage.StartedWhiteBatsWing:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one White Bat's Wing.");
                        }

                            break;
                        
                        case DarkThingsStage.StartedMarauderSpine:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Marauder's Spine.");
                        }

                            break;
                        
                        case DarkThingsStage.StartedKardiFur:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Kardi Fur.");
                        }

                            break;
                        
                        case DarkThingsStage.StartedMimicTeeth:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Mimic Teeth.");
                        }

                            break;
                        
                        case DarkThingsStage.StartedSuccubusHair:
                        {
                            Subject.Reply(source, "You will? Okay, bring me one Succubus's Hair.");
                        }

                            break;
                    }
                }

                break;

            case "darkthings_use":
                source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);

                break;

            case "darkthings_startedspiderseye":

                if (stage == DarkThingsStage.StartedSpidersEye)
                {
                    if (!source.Inventory.HasCount("Spider's Eye", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Spider's Eye", 1);
                    ExperienceDistributionScript.GiveExp(source, tenpercent);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {tenpercent} exp!");
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;

            case "darkthings_startedspiderssilk":
                if (stage == DarkThingsStage.StartedSpidersSilk)
                {
                    if (!source.Inventory.HasCount("Spider's Silk", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Spider's Silk", 1);
                    ExperienceDistributionScript.GiveExp(source, tenpercent);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {tenpercent} exp!");
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;

            case "darkthings_startedcentipedesgland":
                if (stage == DarkThingsStage.StartedCentipedesGland)
                {
                    if (!source.Inventory.HasCount("Centipede's Gland", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Centipede's Gland", 1);
                    ExperienceDistributionScript.GiveExp(source, tenpercent);
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {tenpercent} exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;

            case "darkthings_startedbatswing":
                if (stage == DarkThingsStage.StartedBatsWing)
                {
                    if (!source.Inventory.HasCount("Bat's Wing", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Bat's Wing", 1);
                    ExperienceDistributionScript.GiveExp(source, fifteenpercent);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {fifteenpercent} exp!");
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;
            case "darkthings_startedscorpionsting":
                if (stage == DarkThingsStage.StartedScorpionSting)
                {
                    if (!source.Inventory.HasCount("Scorpion's Sting", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Scorpion's Sting", 1);
                    ExperienceDistributionScript.GiveExp(source, fifteenpercent);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {fifteenpercent} exp!");
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;

            case "darkthings_startedgiantbatswing":
                if (stage == DarkThingsStage.StartedGiantBatsWing)
                {
                    if (!source.Inventory.HasCount("Giant Bat's Wing", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Giant Bat's Wing", 1);
                    ExperienceDistributionScript.GiveExp(source, fifteenpercent);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {fifteenpercent} exp!");
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;

            case "darkthings_startedwhitebatswing":
                if (stage == DarkThingsStage.StartedWhiteBatsWing)
                {
                    if (!source.Inventory.HasCount("White Bat's Wing", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              [Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest])
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("White Bat's Wing", 1);
                    ExperienceDistributionScript.GiveExp(source, twentypercent);
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    source.TryGiveGamePoints(10);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 10 gamepoints and {twentypercent} exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;
            
            case "darkthings_startedmarauderspine":
                if (stage == DarkThingsStage.StartedMarauderSpine)
                {
                    if (!source.Inventory.HasCount("Marauder's Spine", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Marauder's Spine", 1);
                    ExperienceDistributionScript.GiveExp(source, twentypercent);
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    source.TryGiveGamePoints(10);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 10 gamepoints and {twentypercent} exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;
            
            case "darkthings_startedkardifur":
                if (stage == DarkThingsStage.StartedKardiFur)
                {
                    if (!source.Inventory.HasCount("Kardi Fur", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Kardi Fur", 1);
                    ExperienceDistributionScript.GiveExp(source, twentypercent);
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    source.TryGiveGamePoints(10);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 10 gamepoints and {twentypercent} exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;
            
            case "darkthings_startedmimicteeth":
                if (stage == DarkThingsStage.StartedMimicTeeth)
                {
                    if (!source.Inventory.HasCount("Mimic Teeth", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Mimic Teeth", 1);
                    ExperienceDistributionScript.GiveExp(source, twentypercent);
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    source.TryGiveGamePoints(10);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 10 gamepoints and {twentypercent} exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;
            
            case "darkthings_startedsuccubushair":
                if (stage == DarkThingsStage.StartedSuccubusHair)
                {
                    if (!source.Inventory.HasCount("Succubus's Hair", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenpercent);

                    source.Inventory.RemoveQuantity("Succubus's Hair", 1);
                    ExperienceDistributionScript.GiveExp(source, twentypercent);
                    source.Trackers.Enums.Set(DarkThingsStage.None);
                    source.TryGiveGamePoints(10);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 10 gamepoints and {twentypercent} exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("DarkThingsCd", TimeSpan.FromHours(22), true);
                }

                break;
        }
    }
}