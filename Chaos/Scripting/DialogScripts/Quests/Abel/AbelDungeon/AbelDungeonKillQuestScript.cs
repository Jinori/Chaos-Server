using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.Astrid;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests.Abel.AbelDungeon;

public class AbelDungeonKillQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out AbelDungeonKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);

        if (tenPercent > 715000)
            tenPercent = 715000;

        if (source.StatSheet.Level >= 99)
            tenPercent = 10000000;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "nico_initial":
            {
                if (source.UserStatSheet.Level > 71)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "abeldungeonkillquest_initial",
                        OptionText = "Abel Dungeon Slayer"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "abeldungeonkillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("abeldungeoncd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"There is still work to be done. Please come back to me soon. (({cdtime.Remaining.ToReadableString()}))",
                        "nico_initial");

                    return;
                }

                if (!hasStage || (stage == AbelDungeonKillQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "abeldungeonkillquest_start",
                        OptionText = "What can I do?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonSlug))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeonslugcounter", out var slug) || (slug < 10))
                    {
                        Subject.Reply(source, "All you need to do is kill 10 slugs.", "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(source, "Well done. Come back soon.", "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeonslugcounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonGlupe))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeonglupecounter", out var glupe) || (glupe < 10))
                    {
                        Subject.Reply(source, "The Glupes can be annoying to kill, there's 10 I need gone though.", "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(source, "It had to be done. Thank you.", "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeonglupecounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonLeech))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeonleechcounter", out var leech) || (leech < 10))
                    {
                        Subject.Reply(
                            source,
                            "Leechs carry many diseases, it's important we rid 10 of them from the dungeon.",
                            "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(source, "The dungeon is already a safer place, thank you Aisling.", "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeonleechcounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonSpore))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeonsporecounter", out var spore) || (spore < 10))
                    {
                        Subject.Reply(source, "Spores rapidly multiply, exterminate 10 for me.", "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(source, "That'll do the trick, good job Aisling.", "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeonsporecounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonPolyp))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeonpolypcounter", out var polyp) || (polyp < 10))
                    {
                        Subject.Reply(
                            source,
                            "Polyps are quite the creature aren't they? How do they breath? Anyway take out 10 for me.",
                            "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(source, "I knew I could count on you, glad that's over with.", "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeonpolypcounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonDwarf))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeondwarfcounter", out var dwarf) || (dwarf < 10))
                    {
                        Subject.Reply(
                            source,
                            "The dwarfs down there are pretty mean. It's important we eliminate 10 from the dungeon.",
                            "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(
                        source,
                        "The Dwarfs needed to be cut down in size, if they have too many numbers they'll start coming out of the dungeon and we can't have that. Good work.",
                        "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeondwarfcounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonDwarfSoldier))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeondwarfsoldiercounter", out var dwarfsoldier) || (dwarfsoldier < 10))
                    {
                        Subject.Reply(
                            source,
                            "Those Dwarf Soldiers are part of their army, an army too powerful can pose a threat. Please go kill 10 of them.",
                            "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(
                        source,
                        "Those dwarf soldiers get aggressive if they outnumber you, it's good you took care of that.",
                        "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeondwarfsoldiercounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == AbelDungeonKillQuestStage.DungeonBoss))
                {
                    if (!source.Trackers.Counters.TryGetValue("dungeonbosscounter", out var dwarfboss) || (dwarfboss < 1))
                    {
                        Subject.Reply(
                            source,
                            "The Dwarf Captain runs the army, he needs to be kicked down a notch. Go slay him.",
                            "nico_initial");

                        return;
                    }

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, tenPercent);

                    Subject.Reply(
                        source,
                        "The Dwarf Captain was really getting out of hand, it's nice to know you kicked him down a peg. That must've been a difficult task. Thank you Aisling.",
                        "nico_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("abeldungeoncd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dungeonbosscounter", out _);
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.None);
                }

                break;
            }

            case "abeldungeonkillquest_start":
            {
                var roll = new Random().Next(1, 101);

                if (source.UserStatSheet.Level < 85)
                {
                    switch (roll)
                    {
                        case < 33:
                        {
                            source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonSlug);

                            Subject.Reply(
                                source,
                                "Slugs in the dungeon are really annoying, not very dangerous. Go kill 10 Slugs for me.",
                                "Close");

                            return;
                        }
                        case < 66:
                        {
                            Subject.Reply(source, "Glupes can be hard to kill, but I'm sure you'll manage. Kill 10 Glupes.", "Close");
                            source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonGlupe);

                            return;
                        }
                    }

                    Subject.Reply(source, "Leechs are diseased in the dungeon so be careful, slay 10 of the Leechs for me.", "Close");
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonLeech);
                }

                if (source.UserStatSheet.Level is >= 85 and < 97)
                {
                    switch (roll)
                    {
                        case < 33:
                            source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonPolyp);

                            Subject.Reply(
                                source,
                                "Polyps are strange creatures, but populating quickly. Please go kill 10 of them.",
                                "Close");

                            return;
                        case < 66:
                            source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonSpore);
                            Subject.Reply(source, "The Spores in the dungeon are becoming a problem, go take out 10 Spores.", "Close");

                            return;
                    }

                    Subject.Reply(
                        source,
                        "The Dwarfs in the dungeon are becoming overpopulated, it's important we keep their numbers down so they do not attempt to come to the surface. Please take out 10 Dwarfs.",
                        "Close");
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonDwarf);

                    return;
                }

                if (source.UserStatSheet.Level is >= 97 and <= 99)
                {
                    switch (roll)
                    {
                        case < 50:
                            Subject.Reply(
                                source,
                                "The Dwarf Soldiers are part of the dwarf army, with enough numbers they'll try something as we've learned in the past. Please take out 10 Dwarf Soldiers.",
                                "Close");
                            source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonDwarfSoldier);

                            return;
                    }

                    Subject.Reply(
                        source,
                        "The Dwarf Captain runs the dwarf army, with him in power, they may try getting out of the dungeon, it's important we keep him down there. Please slay him.",
                        "Close");
                    source.Trackers.Enums.Set(AbelDungeonKillQuestStage.DungeonBoss);
                }

                break;
            }
        }
    }
}