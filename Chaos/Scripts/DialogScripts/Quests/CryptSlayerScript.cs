using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Quests;

public class CryptSlayerScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public CryptSlayerScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Enums.TryGetValue(out CryptSlayerStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);
        var randomCryptSlayerStage = CryptSlayerStage.None;


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skarn_initial":
            {
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

                    if (source.UserStatSheet.Level is >= 71)
                    {
                        Subject.Text = "Oh my you have grown. You cannot help me here anymore.";
                        source.Enums.Set(CryptSlayerStage.TooHigh);

                        return;
                    }

                    switch (randomCryptSlayerStage)
                    {
                        case CryptSlayerStage.Rat:
                        {
                            Subject.Text = $"Go slay 10 Rats for me.";
                        }

                            break;

                        case CryptSlayerStage.Spider1:
                        {
                            Subject.Text = $"Go slay 10 Spiders for me.";
                        }

                            break;

                        case CryptSlayerStage.Spider2:
                        {
                            Subject.Text = $"Go slay 10 Spiders for me.";
                        }

                            break;
                        case CryptSlayerStage.Centipede1:
                        {
                            Subject.Text = $"Go slay 10 Centipedes for me.";
                        }

                            break;
                        case CryptSlayerStage.Centipede2:
                        {
                            Subject.Text = $"Go slay 10 Centipedes for me.";
                        }

                            break;

                        case CryptSlayerStage.Bat:
                        {
                            Subject.Text = $"Go slay 10 Bats for me.";
                        }

                            break;
                        case CryptSlayerStage.GiantBat:
                        {
                            Subject.Text = $"Go slay 10 Giant Bats for me.";
                        }

                            break;
                        case CryptSlayerStage.Scorpion:
                        {
                            Subject.Text = $"Go slay 10 Scorpions for me.";
                        }

                            break;
                        case CryptSlayerStage.WhiteBat:
                        {
                            Subject.Text = $"Go slay 10 White Bats for me.";
                        }

                            break;
                        case CryptSlayerStage.Kardi:
                        {
                            Subject.Text = $"Go kill 10 Kardis for me.";
                        }

                            break;
                        case CryptSlayerStage.Marauder:
                        {
                            Subject.Text = $"Go kill 10 Murauders for me.";
                        }

                            break;
                        case CryptSlayerStage.Mimic:
                        {
                            Subject.Text = $"Go kill 10 Mimics for me.";
                        }

                            break;
                        case CryptSlayerStage.Succubus:
                        {
                            Subject.Text = $"Go kill 10 Succubus for me.";
                        }

                            break;
                    }
                }

                break;

            case "turnin":
            {
                if (source.Counters.TryGetValue("CryptSlayer", out var value) || (value < 10))
                {
                    source.SendOrangeBarMessage($"Skarn laughs. You haven't killed 10. Go back down there and get to work.");
                    Subject.Close(source);

                    return;
                }
                
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.TryGiveGamePoints(5);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and {twentyPercent} exp!");
                source.Enums.Set(CryptSlayerStage.None);
                Subject.Text = $"Thank you so much for killing those. That's enough for today, come back soon.";

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