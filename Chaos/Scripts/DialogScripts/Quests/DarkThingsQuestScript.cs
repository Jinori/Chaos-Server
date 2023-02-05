using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts.Quests;

public class DarkThingsQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public DarkThingsQuestScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Enums.TryGetValue(out DarkThingsStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);

        var thirtyPercent = Convert.ToInt32(0.30 * tnl);

        var fortyPercent = Convert.ToInt32(0.40 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "dar_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "DarkThings_initial",
                    OptionText = "Dark Things"
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Insert(0, option);
            }

                break;

            case "darkthings_initial":
                if (!hasStage || (stage == DarkThingsStage.None))
                {
                    if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.DarkThingsCd, out var timedEvent))
                    {
                        Subject.Text = $"I have enough for now. Return later. (({timedEvent.Remaining.ToReadableString()}))";

                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "darkthings_yes",
                        OptionText = "Yes I can, what do you need?"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_no",
                        OptionText = "Not right now."
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "darkthings_use",
                        OptionText = "What do you do with these?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(2, option2);
                }

                if (stage == DarkThingsStage.StartedSpidersEye)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedSpidersEye",
                        OptionText = "I have your Spider's Eye here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == DarkThingsStage.StartedCentipedesGland)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedCentipedesGland",
                        OptionText = "I have your Centipede's Gland here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == DarkThingsStage.StartedSpidersSilk)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedSpidersSilk",
                        OptionText = "I have your Spider Silk here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == DarkThingsStage.StartedBatsWing)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedBatsWing",
                        OptionText = "I have your Bat's Wing here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == DarkThingsStage.StartedGreatBatsWing)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedGreatBatsWing",
                        OptionText = "I have your Great Bat's Wing here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == DarkThingsStage.StartedWhiteBatsWing)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedWhiteBatsWing",
                        OptionText = "I have your White Bat's Wing here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    return;
                }

                if (stage == DarkThingsStage.StartedScorpionSting)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "DarkThings_StartedScorpionSting",
                        OptionText = "I have your Scorpion Sting here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "darkthings_where",
                        OptionText = "Where do I find it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);
                }

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

                        source.Enums.Set(randomDarkThingsStage);
                    }

                    if (source.UserStatSheet.Level is >= 11 and < 21)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedSpidersEye, DarkThingsStage.StartedCentipedesGland, DarkThingsStage.StartedSpidersSilk,
                            DarkThingsStage.StartedBatsWing
                        }.PickRandom();

                        source.Enums.Set(randomDarkThingsStage);
                    }

                    if (source.UserStatSheet.Level is >= 21 and < 31)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedBatsWing, DarkThingsStage.StartedCentipedesGland, DarkThingsStage.StartedScorpionSting
                        }.PickRandom();

                        source.Enums.Set(randomDarkThingsStage);
                    }

                    if (source.UserStatSheet.Level is >= 31 and <= 50)
                    {
                        randomDarkThingsStage = new[]
                        {
                            DarkThingsStage.StartedGreatBatsWing, DarkThingsStage.StartedScorpionSting, DarkThingsStage.StartedWhiteBatsWing
                        }.PickRandom();

                        source.Enums.Set(randomDarkThingsStage);
                    }

                    switch (randomDarkThingsStage)
                    {
                        case DarkThingsStage.StartedSpidersEye:
                        {
                            Subject.Text = "You will? Okay, bring me one Spider's Eye.";
                        }

                            break;

                        case DarkThingsStage.StartedSpidersSilk:
                        {
                            Subject.Text = "You will? Okay, bring me one Spider's Silk.";
                        }

                            break;

                        case DarkThingsStage.StartedCentipedesGland:
                        {
                            Subject.Text = "You will? Okay, bring me one Centipede's Gland.";
                        }

                            break;
                        case DarkThingsStage.StartedBatsWing:
                        {
                            Subject.Text = "You will? Okay, bring me one Bat's Wing.";
                        }

                            break;
                        case DarkThingsStage.StartedScorpionSting:
                        {
                            Subject.Text = "You will? Okay, bring me one Scorpion's Sting.";
                        }

                            break;

                        case DarkThingsStage.StartedGreatBatsWing:
                        {
                            Subject.Text = "You will? Okay, bring me one Great Bat's Wing.";
                        }

                            break;
                        case DarkThingsStage.StartedWhiteBatsWing:
                        {
                            Subject.Text = "You will? Okay, bring me one White Bat's Wing";
                        }

                            break;
                    }
                }

                break;

            case "darkthings_use":
                source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(1), true);

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

                    source.Inventory.RemoveQuantity("Spider's Eye", 1);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
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

                    source.Inventory.RemoveQuantity("Spider's Silk", 1);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
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

                    source.Inventory.RemoveQuantity("Centipede's Gland", 1);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
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

                    source.Inventory.RemoveQuantity("Bat's Wing", 1);
                    ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
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

                    source.Inventory.RemoveQuantity("Scorpion's Sting", 1);
                    ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
                }

                break;

            case "darkthings_startedgreatbatswing":
                if (stage == DarkThingsStage.StartedGreatBatsWing)
                {
                    if (!source.Inventory.HasCount("Great Bat's Wing", 1))
                    {
                        source.SendOrangeBarMessage("Dar realizes you have nothing in your hands and scoffs.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("Great Bat's Wing", 1);
                    ExperienceDistributionScript.GiveExp(source, thirtyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
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

                    source.Inventory.RemoveQuantity("White Bat's Wing", 1);
                    ExperienceDistributionScript.GiveExp(source, fortyPercent);
                    source.Enums.Set(DarkThingsStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.DarkThingsCd, TimeSpan.FromHours(8), true);
                }

                break;
        }
    }
}