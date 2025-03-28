using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.MilethQuest;

public class CryptSlayerScript : DialogScriptBase
{
    private readonly IExperienceDistributionScript ExperienceDistributionScript;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<CryptSlayerScript> Logger;

    public CryptSlayerScript(Dialog subject, IItemFactory itemFactory, ILogger<CryptSlayerScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out CryptSlayerStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = MathEx.GetPercentOf<int>(tnl, 10);
        var randomCryptSlayerStage = CryptSlayerStage.None;

        if (tenPercent > 320000)
            tenPercent = 320000;

        if (source.StatSheet.Level >= 99)
            tenPercent = 10000000;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skarn_initial":
                if (source.UserStatSheet.BaseClass is BaseClass.Peasant)
                {
                    Subject.Reply(source, "You cannot help me until you've dedicated yourself to a class.");

                    return;
                }

                if (stage == CryptSlayerStage.Completed)
                {
                    Subject.Reply(source, "Thanks for all your hard work Aisling, we can keep these creatures where they belong.");

                    return;
                }

                if (source.Trackers.Counters.TryGetValue("CryptSlayerLegend", out var legend) && (legend >= 10))
                {
                    source.Trackers.Enums.Set(CryptSlayerStage.Completed);
                    source.Trackers.Flags.AddFlag(LanternSizes.LargeLantern);
                    source.Legend.Remove("CryptSlayer", out _);

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Controlled the Mileth Crypt Population",
                            "CryptSlayerCompleted",
                            MarkIcon.Victory,
                            MarkColor.Blue,
                            1,
                            GameTime.Now));

                    Subject.Reply(
                        source,
                        "Thanks for all your hard work Aisling, we can keep these creatures where they belong. Here's a Large Lantern for your troubles.");
                }

                break;

            case "cryptslayer_initial":
                if (!hasStage || (stage == CryptSlayerStage.None))
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("CryptSlayerCd", out var timedEvent))
                    {
                        Subject.Reply(
                            source,
                            $"You have killed enough for now, come back later. (({timedEvent.Remaining.ToReadableString()}))");

                        return;
                    }

                    if (stage == CryptSlayerStage.Completed)
                    {
                        Subject.Reply(source, "Thank you again Aisling, the crypt is a safer place now.");

                        return;
                    }
                }

                if (hasStage)
                    Subject.Reply(source, "Skip", "cryptslayer_turninstart");

                break;

            case "cryptslayer_start":
                if (!hasStage || (stage == CryptSlayerStage.None))
                {
                    if (source.UserStatSheet.Level < 11)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Rat,
                            CryptSlayerStage.Centipede1,
                            CryptSlayerStage.Spider1
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 11 and < 21)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Centipede2,
                            CryptSlayerStage.Bat,
                            CryptSlayerStage.Scorpion,
                            CryptSlayerStage.Spider2
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 21 and < 31)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Scorpion,
                            CryptSlayerStage.Bat,
                            CryptSlayerStage.GiantBat
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level is >= 31 and <= 50)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.WhiteBat,
                            CryptSlayerStage.Mimic,
                            CryptSlayerStage.GiantBat
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    if (source.UserStatSheet.Level >= 50)
                    {
                        randomCryptSlayerStage = new[]
                        {
                            CryptSlayerStage.Succubus,
                            CryptSlayerStage.Marauder,
                            CryptSlayerStage.Kardi
                        }.PickRandom();

                        source.Trackers.Enums.Set(randomCryptSlayerStage);
                    }

                    switch (randomCryptSlayerStage)
                    {
                        case CryptSlayerStage.Rat:
                            Subject.Reply(source, "I need you to kill 10 Rats, this will keep the population down.");

                            break;

                        case CryptSlayerStage.Spider1:
                            Subject.Reply(source, "Please go kill 10 Spiders on the upper floors, they are reproducing quickly.");

                            break;

                        case CryptSlayerStage.Spider2:
                            Subject.Reply(source, "Please go kill 10 Spiders on the lower floors, they are reproducing quickly.");

                            break;

                        case CryptSlayerStage.Centipede1:
                            Subject.Reply(source, "Handle 10 Centipedes for me, the ones found on the upper floors.");

                            break;

                        case CryptSlayerStage.Centipede2:
                            Subject.Reply(source, "Handle 10 Centipedes for me, the ones found on the lower floors.");

                            break;

                        case CryptSlayerStage.Bat:
                            Subject.Reply(source, "Seems to be Bat season, can you kill 10 Bats for me?");

                            break;

                        case CryptSlayerStage.GiantBat:
                            Subject.Reply(source, "The Giant bats are out of control, please clear 10 of them for me.");

                            break;

                        case CryptSlayerStage.Scorpion:
                            Subject.Reply(source, "Scorpions are overpopulated, please go kill 10 Scorpions.");

                            break;

                        case CryptSlayerStage.WhiteBat:
                            Subject.Reply(source, "Please kill 10 White Bats, they're invasive.");

                            break;

                        case CryptSlayerStage.Kardi:
                            Subject.Reply(source, "Travel deep, kill 10 Kardis for me, annoying little things.");

                            break;

                        case CryptSlayerStage.Marauder:
                            Subject.Reply(
                                source,
                                "Marauders are really interesting but there's too many. Please kill 10 Marauders for me.");

                            break;

                        case CryptSlayerStage.Mimic:
                            Subject.Reply(source, "Tricky little beasts, these mimics. Way too many of them, kill 10 Mimics.");

                            break;

                        case CryptSlayerStage.Succubus:
                            Subject.Reply(
                                source,
                                "Beautiful Succubus, but so deadly. They'll be roaming with us soon if we don't clear them. Please kill 10 Succubus.");

                            break;
                    }
                }

                break;

            case "cryptslayer_turnin":
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
                            source.SendOrangeBarMessage("You haven't killed 10 Marauders. Get back to work.");

                            break;
                    }

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("CryptSlayerLegend");

                if (source.Trackers.Counters.CounterLessThanOrEqualTo("CryptSlayerLegend", 1))
                {
                    source.Trackers.Flags.AddFlag(LanternSizes.SmallLantern);
                    source.Trackers.Flags.AddFlag(RionaTutorialQuestFlags.Skarn);
                    source.SendOrangeBarMessage("Skarn hands you a weapon and a Small Lantern (Hit F1).");

                    if (source.HasClass(BaseClass.Wizard))
                    {
                        var wizardstaff = ItemFactory.Create("MagusAres");
                        source.GiveItemOrSendToBank(wizardstaff);
                    }

                    if (source.HasClass(BaseClass.Warrior))
                    {
                        var warriorweapon = ItemFactory.Create("Claidheamh");
                        source.GiveItemOrSendToBank(warriorweapon);
                    }

                    if (source.HasClass(BaseClass.Priest))
                    {
                        var prieststaff = ItemFactory.Create("HolyHermes");
                        source.GiveItemOrSendToBank(prieststaff);
                    }

                    if (source.HasClass(BaseClass.Rogue))
                    {
                        var rogueweapon = ItemFactory.Create("BlossomDagger");
                        source.GiveItemOrSendToBank(rogueweapon);
                    }

                    if (source.HasClass(BaseClass.Monk))
                    {
                        var monkweapon = ItemFactory.Create("WolfClaw");
                        source.GiveItemOrSendToBank(monkweapon);
                    }
                }

                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest
                          ])
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                ExperienceDistributionScript.GiveExp(source, tenPercent);
                source.TryGiveGamePoints(5);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive 5 gamepoints and {tenPercent} exp!");
                source.Trackers.Enums.Remove(typeof(CryptSlayerStage));
                Subject.Reply(source, "Thank you so much for killing those. That's enough for today, come back soon.");
                source.Trackers.Counters.Remove("CryptSlayer", out _);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Helped Skarn clear the Mileth Crypt",
                        "CryptSlayer",
                        MarkIcon.Victory,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                source.Trackers.TimedEvents.AddEvent("CryptSlayerCd", TimeSpan.FromHours(4), true);
                var skarn = Subject.DialogSource as Merchant;
                skarn?.Say($"Thanks for the help, {source.Name}!");

                break;
        }
    }
}