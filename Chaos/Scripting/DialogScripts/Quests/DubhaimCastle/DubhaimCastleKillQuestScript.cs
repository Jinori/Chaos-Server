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

namespace Chaos.Scripting.DialogScripts.Quests.DubhaimCastle;

public class DubhaimCastleKillQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out DubhaimCastleKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);

        if (tenPercent > 320000)
            tenPercent = 1000000;

        if (source.UserStatSheet.Level >= 99)
            tenPercent = 10000000;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "hosk_initial":
            {
                if (source.UserStatSheet.Level < 11)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dubhaimcastlekillquest_initial",
                    OptionText = "Dubhaim Castle Slayer"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "dubhaimcastlekillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dubhaimcastlecd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"That makes me happy. Please return to me soon. (Time remaining: {cdtime.Remaining.ToReadableString()})",
                        "hosk_initial");

                    return;
                }

                if (!hasStage || (stage == DubhaimCastleKillQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "dubhaimcastlekillquest_start",
                        OptionText = "What can I do?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimDunan))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimdunancounter", out var dunan) || (dunan < 10))
                    {
                        Subject.Reply(source, "You need to be stronger than this. Come back after you've slain 10 Dunan.", "hosk_initial");

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

                    Subject.Reply(source, "Well done. Come back soon.", "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimdunancounter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimGhast))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimghastcounter", out var ghast) || (ghast < 10))
                    {
                        Subject.Reply(source, "The ghast are formidable opponents sure, handle 10 of them.", "hosk_initial");

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

                    Subject.Reply(source, "A job well done. Thank you Aisling.", "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimghastcounter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimCruel1))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimcruel1counter", out var Cruel1) || (Cruel1 < 10))
                    {
                        Subject.Reply(
                            source,
                            "Cruels are very tough beings. I understand your struggle. You must take down 10 of these Cruels.",
                            "hosk_initial");

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

                    Subject.Reply(source, "The Cruels will show no mercy, they'll be back stronger.", "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimcruel1counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimCruel2))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimcruel2counter", out var Cruel2) || (Cruel2 < 10))
                    {
                        Subject.Reply(
                            source,
                            "There are some aggressive Cruels in the hidden rooms of the castle, please go kill 10 of them.",
                            "hosk_initial");

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

                    Subject.Reply(source, "The Cruels will show no mercy, they'll be back stronger.", "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimcruel2counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimGargoyle1))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimgargoyle1counter", out var gargoyle1) || (gargoyle1 < 10))
                    {
                        Subject.Reply(
                            source,
                            "These well maneuvering beast in the courtyard must remain flightless, eliminate 10 of those Gargoyles.",
                            "hosk_initial");

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

                    Subject.Reply(source, "That's better. The courtyard should be for all to enjoy.", "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimgargoyle1counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimGargoyle2))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimgargoyle2counter", out var gargoyle2) || (gargoyle2 < 10))
                    {
                        Subject.Reply(
                            source,
                            "In the hidden places of the castle, there are gargoyles that must be defeated. Take down 10 of them.",
                            "hosk_initial");

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
                        "It's a difficult task I know, you succeeded through your battle though. Great work Aisling.",
                        "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimgargoyle2counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimGargoyleFiend1))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimgargoylefiend1counter", out var gargoylefiend1)
                        || (gargoylefiend1 < 10))
                    {
                        Subject.Reply(
                            source,
                            "In the hidden parts of the castle, roam too many Gargoyle Fiends, please take down 10 of them.",
                            "hosk_initial");

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
                        "I knew you could do it Aisling. Those Gargoyle Fiends won't be flying through our halls.",
                        "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimgargoylefiend1counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimGargoyle3))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimgargoyle3counter", out var gargoyle3) || (gargoyle3 < 10))
                    {
                        Subject.Reply(
                            source,
                            "The Gargoyles in the basement must be removed, please go slaughter 10 of them.",
                            "hosk_initial");

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
                        "The Gargoyles in the basement are a pain to deal with but a necessity to deal with. Good work.",
                        "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimgargoyle3counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);

                    return;
                }

                if (hasStage && (stage == DubhaimCastleKillQuestStage.DubhaimGargoyleFiend2))
                {
                    if (!source.Trackers.Counters.TryGetValue("dubhaimgargoylefiend2counter", out var gargoylefiend2)
                        || (gargoylefiend2 < 10))
                    {
                        Subject.Reply(
                            source,
                            "The Gargoyle Fiends in the basement do whatever they please, they must be stopped. Take down 10 Gargoyle Fiends in the basement.",
                            "hosk_initial");

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
                        "It was a dangerous task to ask of you Aisling, but you did it so well. It's all I could ask for. Thank you.",
                        "hosk_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("dubhaimcastlecd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("dubhaimgargoylefiend2counter", out _);
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.None);
                }

                break;
            }

            case "dubhaimcastlekillquest_start":
            {
                var roll = new Random().Next(1, 101);

                if (source.UserStatSheet.Level < 41)
                    switch (roll)
                    {
                        case < 33:
                            source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimDunan);

                            Subject.Reply(
                                source,
                                "The Dunan population right inside is growing fast. Help me by eliminating 10 Dunans.",
                                "Close");

                            return;
                        case < 66:
                            Subject.Reply(
                                source,
                                "The Ghast inside the castle are becoming a concern. Please go kill 10 of the Ghast.",
                                "Close");
                            source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimGhast);

                            return;
                        default:
                            Subject.Reply(
                                source,
                                "The Cruels on the first floor inside the castle are taking over. Let's put them in their place, kill 10 of the Cruels.",
                                "Close");
                            source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimCruel1);

                            break;
                    }

                if (source.UserStatSheet.Level is >= 41 and < 60)
                {
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimGargoyle1);

                    Subject.Reply(
                        source,
                        "There's secret rooms inside the castle, in those rooms are flying Gargoyles that are problematic. Please go eliminate 10 of these Gargoyles.",
                        "Close");

                    return;
                }

                if (source.UserStatSheet.Level is >= 60 and < 80)
                {
                    switch (roll)
                    {
                        case < 33:
                            source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimGargoyle2);

                            Subject.Reply(
                                source,
                                "There's secret rooms inside the castle, in those rooms are flying Gargoyles that are problematic. Please go eliminate 10 of these Gargoyles.",
                                "Close");

                            return;
                        case < 66:
                            source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimCruel2);

                            Subject.Reply(
                                source,
                                "The Cruels inside the hidden rooms that are quickly learning from the Gargoyles are becoming an issue, please go kill 10 of the Cruels.",
                                "Close");

                            return;
                    }

                    Subject.Reply(
                        source,
                        "Gargoyle Fiends in the hidden areas of the castle are acting up. I need you to go handle 10 of these Gargoyle Fiends.",
                        "Close");
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimGargoyleFiend1);

                    return;
                }

                if (source.UserStatSheet.Level >= 80)
                {
                    switch (roll)
                    {
                        case < 50:
                            Subject.Reply(
                                source,
                                "There are Gargoyles in the basement becoming overly aggressive. To prevent any further aggression, please go slay 10 of these Gargoyles.",
                                "Close");
                            source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimGargoyle3);

                            return;
                    }

                    Subject.Reply(
                        source,
                        "Many of the Gargoyle Fiends in the basement are threatening to escape. Let's show them they can't handle the forces outside of the castle. Please go slay 10 of these Gargoyle Fiends.",
                        "Close");
                    source.Trackers.Enums.Set(DubhaimCastleKillQuestStage.DubhaimGargoyleFiend2);
                }

                break;
            }
        }
    }
}