using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests;

public class CryptSlayerScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public CryptSlayerScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = MathEx.GetPercentOf<int>(tnl, 20);
        var randomCryptSlayerStage = CryptSlayerStage.None;


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skarn_initial":
            {
                if (source.UserStatSheet.BaseClass is not BaseClass.Monk or BaseClass.Priest or BaseClass.Warrior or BaseClass.Rogue
                                                      or BaseClass.Wizard)
                {
                    Subject.Reply(source, "You cannot help me until you've dedicated yourself to a class.");
                    return;
                }

                if (source.UserStatSheet.Level > 71)
                {
                    Subject.Reply(source, "You're an experienced Aisling, I have nothing for you.");
                    return;
                }

                if (stage == CryptSlayerStage.Completed)
                {
                    Subject.Reply(source, "Thanks for all your hard work Aisling, we can keep these creatures where they belong.");

                    return;
                }

                if (source.Trackers.Counters.TryGetValue("CryptSlayerLegend", out var value) && (value >= 10))
                {
                    source.Trackers.Enums.Set(CryptSlayerStage.Completed);
                    source.Legend.Remove("CryptSlayer", out _);
                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Controlled the Mileth Crypt population.",
                            "CryptSlayerCompleted",
                            MarkIcon.Victory,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));

                    Subject.Reply(source, "Thanks for all your hard work Aisling, we can keep these creatures where they belong.");

                    return;
                }
            }

                break;

            case "cryptslayer_initial":
                if (!hasStage || (stage == CryptSlayerStage.None))
                {
                    if (source.UserStatSheet.Level > 71)
                    {
                        Subject.Reply(source,"I have no quest for an experienced player like yourself.");
                        return;
                    }

                    if (source.Trackers.TimedEvents.HasActiveEvent("CryptSlayerCd", out var timedEvent))
                    {
                        Subject.Reply(source, $"You have killed enough for now, come back later. (({timedEvent.Remaining.ToReadableString()}))");

                        return;
                    }
                    
                    if (stage == CryptSlayerStage.Completed)
                    {
                        Subject.Reply(source, "Thank you again Aisling, the crypt is a safer place now.");

                        return;
                    }
                }

                if (hasStage)
                {
                    Subject.Reply(source, "Skip", "cryptslayer_turninstart");

                    return;
                }

                break;

            case "cryptslayer_start":
                if (!hasStage || (stage == CryptSlayerStage.None))
                {

                    if (source.UserStatSheet.Level < 11)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Rat, CryptSlayerStage.Centipede1, CryptSlayerStage.Spider1
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 11 and < 21)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Centipede2, CryptSlayerStage.Bat, CryptSlayerStage.Scorpion, CryptSlayerStage.Spider2
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 21 and < 31)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Scorpion, CryptSlayerStage.Bat, CryptSlayerStage.GiantBat, CryptSlayerStage.WhiteBat
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 31 and <= 50)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.WhiteBat, CryptSlayerStage.Mimic, CryptSlayerStage.GiantBat, CryptSlayerStage.Marauder
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 50 and <= 71)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Succubus, CryptSlayerStage.Mimic, CryptSlayerStage.Marauder, CryptSlayerStage.Kardi
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    switch (randomCryptSlayerStage)
                    {
                        case CryptSlayerStage.Rat:
                        {
                            Subject.Reply(source, "I need you to kill 10 Rats, this will keep the population down.");
                        }

                            break;

                        case CryptSlayerStage.Spider1:
                        {
                            Subject.Reply(source, "Please go kill 10 Spiders on the upper floors, they are reproducing quickly.");
                        }

                            break;

                        case CryptSlayerStage.Spider2:
                        {
                            Subject.Reply(source, "Please go kill 10 Spiders on the lower floors, they are reproducing quickly.");
                        }

                            break;
                        case CryptSlayerStage.Centipede1:
                        {
                            Subject.Reply(source, "Handle 10 Centipedes for me, the ones found on the upper floors.");
                        }

                            break;
                        case CryptSlayerStage.Centipede2:
                        {
                            Subject.Reply(source, "Handle 10 Centipedes for me, the ones found on the lower floors.");
                        }

                            break;

                        case CryptSlayerStage.Bat:
                        {
                            Subject.Reply(source, "Seems to be Bat season, can you kill 10 Bats for me?");
                        }

                            break;
                        case CryptSlayerStage.GiantBat:
                        {
                            Subject.Reply(source, "The Giant bats are out of control, please clear 10 of them for me.");
                        }

                            break;
                        case CryptSlayerStage.Scorpion:
                        {
                            Subject.Reply(source, "Scorpions are over populated, please go kill 10 Scorpions.");
                        }

                            break;
                        case CryptSlayerStage.WhiteBat:
                        {
                            Subject.Reply(source, "Please kill 10 White Bats, they're invasive.");
                        }

                            break;
                        case CryptSlayerStage.Kardi:
                        {
                            Subject.Reply(source, "Travel deep, kill 10 Kardis for me, annoying little things.");
                        }

                            break;
                        case CryptSlayerStage.Marauder:
                        {
                            Subject.Reply(source, "Marauders are really interesting but there's too many. Please kill 10 Marauders for me.");
                        }

                            break;
                        case CryptSlayerStage.Mimic:
                        {
                            Subject.Reply(source, "Tricky little beast, these mimics. Way too many of them, kill 10 Mimics.");
                        }

                            break;
                        case CryptSlayerStage.Succubus:
                        {
                            Subject.Reply(source, "Beautiful Succubus, but so deadly. They'll be roaming with us soon if we don't clear them. Please kill 10 Succubus.");
                        }

                            break;
                    }
                }

                break;

            case "cryptslayer_turnin":
            {
                if (!source.Trackers.Counters.TryGetValue("CryptSlayer", out var value) || (value < 10))
                {
                    Subject.Close(source);
                    
                    switch (stage)
                    {
                        case CryptSlayerStage.Rat:
                            source.SendOrangeBarMessage("You haven't killed 10 Rats. Get back to work.");

                            break;
                        case CryptSlayerStage.Bat:
                            source.SendOrangeBarMessage("You haven't killed 10 Bats. Get back to work.");

                            break;
                        case CryptSlayerStage.Spider1:
                        case CryptSlayerStage.Spider2:
                            source.SendOrangeBarMessage("You haven't killed 10 Spiders. Get back to work.");

                            break;
                        case CryptSlayerStage.Centipede1:
                        case CryptSlayerStage.Centipede2:
                            source.SendOrangeBarMessage("You haven't killed 10 Centipedes. Get back to work.");

                            break;
                        case CryptSlayerStage.Scorpion:
                            source.SendOrangeBarMessage("You haven't killed 10 Scorpions. Get back to work.");

                            break;
                        case CryptSlayerStage.GiantBat:
                            source.SendOrangeBarMessage("You haven't killed 10 Giant Bats. Get back to work.");

                            break;
                        case CryptSlayerStage.WhiteBat:
                            source.SendOrangeBarMessage("You haven't killed 10 White Bats. Get back to work.");

                            break;
                        case CryptSlayerStage.Mimic:
                            source.SendOrangeBarMessage("You haven't killed 10 Mimics. Get back to work.");

                            break;
                        case CryptSlayerStage.Kardi:
                            source.SendOrangeBarMessage("You haven't killed 10 Kardis. Get back to work.");

                            break;
                        case CryptSlayerStage.Succubus:
                            source.SendOrangeBarMessage("You haven't killed 10 Succubus. Get back to work.");

                            break;
                        case CryptSlayerStage.Marauder:
                            source.SendOrangeBarMessage("You haven't killed 10 Marauder. Get back to work.");

                            break;
                    }

                    return;
                }
                
                
                source.Trackers.Counters.AddOrIncrement("CryptSlayerLegend");

                if (source.Trackers.Counters.CounterLessThanOrEqualTo("CryptSlayerLegend", 1))
                {
                    if (source.HasClass(BaseClass.Wizard))
                    {
                        var wizardstaff = ItemFactory.Create("MagusAres");
                        source.TryGiveItem(wizardstaff);
                    }

                    if (source.HasClass(BaseClass.Warrior))
                    {
                        var warriorweapon = ItemFactory.Create("Claidheamh");
                        source.TryGiveItem(warriorweapon);
                    }

                    if (source.HasClass(BaseClass.Priest))
                    {
                        var prieststaff = ItemFactory.Create("HolyHermes");
                        source.TryGiveItem(prieststaff);
                    }

                    if (source.HasClass(BaseClass.Rogue))
                    {
                        var rogueweapon = ItemFactory.Create("BlossomDagger");
                        source.TryGiveItem(rogueweapon);
                    }

                    if (source.HasClass(BaseClass.Monk))
                    {
                        var monkweapon = ItemFactory.Create("WolfClaw");
                        source.TryGiveItem(monkweapon);
                    }
                }
                
                source.SendOrangeBarMessage("Skarn hands you a weapon.");

                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.TryGiveGamePoints(5);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and {twentyPercent} exp!");
                source.Trackers.Enums.Remove(typeof(CryptSlayerStage));
                Subject.Reply(source, "Thank you so much for killing those. That's enough for today, come back soon.");
                source.Trackers.Counters.Remove("CryptSlayer", out _);
                source.Trackers.Counters.AddOrIncrement("CryptSlayerLegend", 1);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Helped Skarn clear the Mileth Crypt",
                        "CryptSlayer",
                        MarkIcon.Victory,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.Trackers.TimedEvents.AddEvent("CryptSlayerCd",TimeSpan.FromSeconds(4), true);

                break;
            }
        }
    }
    }