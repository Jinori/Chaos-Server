using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories;
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
        var hasStage = source.Enums.TryGetValue(out CryptSlayerStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = MathEx.GetPercentOf<int>(tnl, 20);
        var randomCryptSlayerStage = CryptSlayerStage.None;


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skarn_initial":
            {

                if (source.UserStatSheet.Level > 71)
                {
                    Subject.Text = "You're an experienced Aisling, I have nothing for you.";
                    return;
                }

                if (stage == CryptSlayerStage.Completed)
                {
                    Subject.Text = "Thanks for all your hard work Aisling, we can keep these creatures where they belong.";

                    return;
                }

                if (source.Counters.TryGetValue("CryptSlayerLegend", out var value) && (value >= 10))
                {
                    source.Enums.Set(CryptSlayerStage.Completed);
                    source.Legend.Remove("CryptSlayer", out _);
                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Controlled the Mileth Crypt population with Skarn.",
                            "CryptSlayerCompleted",
                            MarkIcon.Victory,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));

                    Subject.Text = "Thanks for all your hard work Aisling, we can keep these creatures where they belong.";

                    return;
                }
                
                var option = new DialogOption
                {
                    DialogKey = "cryptslayer_initial",
                    OptionText = "Slayer of the Crypt"
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Insert(0, option);
            }

                break;

            case "cryptslayer_initial":
                if (!hasStage || (stage == CryptSlayerStage.None))
                {
                    if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.CryptSlayerCd, out var timedEvent))
                    {
                        Subject.Text = $"You have killed enough for now, come back later. (({timedEvent.Remaining.ToReadableString()}))";

                        return;
                    }
                    
                    if (stage == CryptSlayerStage.Completed)
                    {
                        Subject.Text = "Thank you again Aisling, the crypt is a safer place now.";

                        return;
                    }
                    
                    var option = new DialogOption
                    {
                        DialogKey = "cryptslayer_start",
                        OptionText = "Yes"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "cryptslayer_deny",
                        OptionText = "No thanks"
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "cryptslayer_who",
                        OptionText = "Who are you?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(2, option2);
                }

                if (hasStage)
                {

                    Subject.Text = "Did you have any issues?";

                    var option = new DialogOption
                    {
                        DialogKey = "cryptslayer_turnin",
                        OptionText = "I cleared them all."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
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

                        source.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 11 and < 21)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Centipede2, CryptSlayerStage.Bat, CryptSlayerStage.Scorpion, CryptSlayerStage.Spider2
                        }.PickRandom();

                        source.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 21 and < 31)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Scorpion, CryptSlayerStage.Bat, CryptSlayerStage.GiantBat, CryptSlayerStage.WhiteBat
                        }.PickRandom();

                        source.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 31 and <= 50)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.WhiteBat, CryptSlayerStage.Mimic, CryptSlayerStage.GiantBat, CryptSlayerStage.Marauder
                        }.PickRandom();

                        source.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 50 and <= 71)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Succubus, CryptSlayerStage.Mimic, CryptSlayerStage.Marauder, CryptSlayerStage.Kardi
                        }.PickRandom();

                        source.Enums.Set(randomCryptSlayerStage);
                    }

                    switch (randomCryptSlayerStage)
                    {
                        case CryptSlayerStage.Rat:
                        {
                            Subject.Text = "I need you to kill 10 Rats, this will keep the population down.";
                        }

                            break;

                        case CryptSlayerStage.Spider1:
                        {
                            Subject.Text = "Please go kill 10 Spiders on the upper floors, they are reproducing quickly.";
                        }

                            break;

                        case CryptSlayerStage.Spider2:
                        {
                            Subject.Text = "Please go kill 10 Spiders on the lower floors, they are reproducing quickly.";
                        }

                            break;
                        case CryptSlayerStage.Centipede1:
                        {
                            Subject.Text = "Handle 10 Centipedes for me, the ones found on the upper floors.";
                        }

                            break;
                        case CryptSlayerStage.Centipede2:
                        {
                            Subject.Text = "Handle 10 Centipedes for me, the ones found on the lower floors.";
                        }

                            break;

                        case CryptSlayerStage.Bat:
                        {
                            Subject.Text = "Seems to be Bat season, can you kill 10 Bats for me?";
                        }

                            break;
                        case CryptSlayerStage.GiantBat:
                        {
                            Subject.Text = "The Giant bats are out of control, please clear 10 of them for me.";
                        }

                            break;
                        case CryptSlayerStage.Scorpion:
                        {
                            Subject.Text = "Scorpions are over populated, please go kill 10 Scorpions.";
                        }

                            break;
                        case CryptSlayerStage.WhiteBat:
                        {
                            Subject.Text = "Please kill 10 White Bats, they're invasive.";
                        }

                            break;
                        case CryptSlayerStage.Kardi:
                        {
                            Subject.Text = "Travel deep, kill 10 Kardis for me, annoying little things.";
                        }

                            break;
                        case CryptSlayerStage.Marauder:
                        {
                            Subject.Text = "Marauders are really interesting but there's too many. Please kill 10 Marauders for me.";
                        }

                            break;
                        case CryptSlayerStage.Mimic:
                        {
                            Subject.Text = "Tricky little beast, these mimics. Way too many of them, kill 10 Mimics.";
                        }

                            break;
                        case CryptSlayerStage.Succubus:
                        {
                            Subject.Text = "Beautiful Succubus, but so deadly. They'll be roaming with us soon if we don't clear them. Please kill 10 Succubus.";
                        }

                            break;
                    }
                }

                break;

            case "cryptslayer_turnin":
            {
                if (!source.Counters.TryGetValue("CryptSlayer", out var value) || (value < 10))
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

                var wizardstaff = ItemFactory.Create("MagusZeus");
                var prieststaff = ItemFactory.Create("HolyAres");
                var monkweapon = ItemFactory.Create("WolfClaws");
                var rogueweapon = ItemFactory.Create("BlossomDagger");
                var warriorshield = ItemFactory.Create("LeatherShield");

                if (source.Counters.CounterLessThanOrEqualTo("CryptSlayerLegend", 0))
                {

                    if (source.UserStatSheet.BaseClass.Equals("Wizard"))
                    {
                        source.TryGiveItem(wizardstaff);
                    }

                    if (source.UserStatSheet.BaseClass.Equals("Warrior"))
                    {
                        source.TryGiveItem(warriorshield);
                    }

                    if (source.UserStatSheet.BaseClass.Equals("Priest"))
                    {
                        source.TryGiveItem(prieststaff);
                    }

                    if (source.UserStatSheet.BaseClass.Equals("Rogue"))
                    {
                        source.TryGiveItem(rogueweapon);
                    }

                    if (source.UserStatSheet.BaseClass.Equals("monk"))
                    {
                        source.TryGiveItem(monkweapon);
                    }
                }

                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.TryGiveGamePoints(5);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and {twentyPercent} exp!");
                source.Enums.Remove(typeof(CryptSlayerStage));
                Subject.Text = "Thank you so much for killing those. That's enough for today, come back soon.";
                source.Counters.Remove("CryptSlayer", out _);
                source.Counters.AddOrIncrement("CryptSlayerLegend", 1);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Helped Skarn clear the Mileth Crypt",
                        "CryptSlayer",
                        MarkIcon.Victory,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.TimedEvents.AddEvent(TimedEvent.TimedEventId.CryptSlayerCd, TimeSpan.FromHours(4), true);

                break;
            }
        }
    }
    }