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

namespace Chaos.Scripting.DialogScripts.Quests.PietSewer;

public class PietSewerKillQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out PietSewerKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);
        
        if (tenPercent > 320000)
        {
            tenPercent = 320000;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "waldyr_initial":
            {
                if (source.UserStatSheet.Level < 99)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "pietsewerkillquest_initial",
                        OptionText = "Piet Sewer Slayer"
                    };
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "pietsewerkillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("pietsewercd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"That pleases me. Please come back to me soon. (({cdtime.Remaining.ToReadableString()}))",
                        "waldyr_initial");
                    return;
                }

                if (!hasStage || stage == PietSewerKillQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "pietsewerkillquest_start",
                        OptionText = "What can I do?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                    return;
                }

                if (hasStage && stage == PietSewerKillQuestStage.SewerCrab)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewercrabcounter", out var crab) || crab < 10)
                    {
                        Subject.Reply(source, "All you need to do is kill 10 crabs.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Well done. Come back soon.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewercrabcounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == PietSewerKillQuestStage.SewerTurtle)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerturtlecounter", out var turtle) || turtle < 10)
                    {
                        Subject.Reply(source, "Turtles here are soft, go kill 10 and return to me.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"A job well done. Thank you Aisling.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerturtlecounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == PietSewerKillQuestStage.SewerFrog)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerfrogcounter", out var frog) || frog < 10)
                    {
                        Subject.Reply(source, "Frogs here tend to be annoying, get down there and kill 10.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"The frogs are all over, that didn't take you very long, thank you.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerfrogcounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == PietSewerKillQuestStage.SewerAnemone)
                {
                    if (!source.Trackers.Counters.TryGetValue("seweranemonecounter", out var anemone) || anemone < 10)
                    {
                        Subject.Reply(source, "Anemones are ruining the ecosystem down there, slay 10 of them for me.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Much better, those things are really bothersome.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("seweranemonecounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == PietSewerKillQuestStage.SewerBrawlfish)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerbrawlfishcounter", out var spore) || spore < 10)
                    {
                        Subject.Reply(source, "The brawlfish can be very difficult, stubborn little monsters. Kill 10 of them, show them who's boss.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Not so easy was it? Maybe you're just stronger than me. I appreciate the effort you put into slaying them.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerbrawlfishcounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == PietSewerKillQuestStage.SewerRockCobbler)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerrockcobblercounter", out var rockcobbler) || rockcobbler < 10)
                    {
                        Subject.Reply(source, "Rock Cobblers are very defensive creatures, killing them can be pretty tough. But you seem tough yourself, go ahead and handle 10 of them.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"I figured you'd find a way to break through their body. It's not so easy but you did it.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerrockcobblercounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == PietSewerKillQuestStage.SewerKraken)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerkrakencounter", out var kraken) || kraken < 10)
                    {
                        Subject.Reply(source, "Krakens down in the sewers are a strange sight, but they love it and it's difficult to get them out of there. Keep at it, only need to kill 10.", "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Must be nice being that strong to handle Krakens so easily. I knew you could do it.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerkrakencounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == PietSewerKillQuestStage.SewerGremlin)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewergremlincounter", out var gremlin) || gremlin < 10)
                    {
                        Subject.Reply(source,
                            "Tricky little Gremlins. Can you outsmart them? Take down 10 of those Gremlins.",
                            "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"The Gremlins in this sewer are not good for it. I knew I picked the right Aisling to handle that problem.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewergremlincounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == PietSewerKillQuestStage.SewerGog)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewergogcounter", out var gog) || gog < 10)
                    {
                        Subject.Reply(source,
                            "Tricky little Gremlins. Can you outsmart them? Take down 10 of those Gremlins.",
                            "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"The Gremlins in this sewer are not good for it. I knew I picked the right Aisling to handle that problem.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewergremlincounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == PietSewerKillQuestStage.SewerMiniSkrull)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerminiskrullcounter", out var miniskrull) || miniskrull < 5)
                    {
                        Subject.Reply(source,
                            "The Mini Skrulls don't like intruders, I'm surprised you made it back alive. We need to get rid of 5 of those Mini Skrulls.",
                            "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"They put up a good fight I'm sure, you look like you took a beating. In the end, you came out victorious, great work.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerminiskrullcounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == PietSewerKillQuestStage.SewerSkrull)
                {
                    if (!source.Trackers.Counters.TryGetValue("sewerskrullcounter", out var skrull) || skrull < 3)
                    {
                        Subject.Reply(source,
                            "The big Skrull at the bottom is terrifying, I have to escape everytime he is there. He sure has made himself a home. Can you defeat him three times?",
                            "waldyr_initial");
                        return;
                    }

                    logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"I saw your fight with the Skrull, that was shocking! The way he fell warms my heart Aisling.",
                        "waldyr_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("pietsewercd", TimeSpan.FromHours(24), true);
                    source.Trackers.Counters.Remove("sewerskrullcounter", out _);
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.None);
                }

                break;
            }

            case "pietsewerkillquest_start":
            {
                var roll = new Random().Next(1, 101);
                
                if (source.UserStatSheet.Level < 50)
                {
                    switch (roll)
                    {
                        case < 25:
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerCrab);
                            Subject.Reply(source,
                                "Ah, this task is easy, go defeat 10 crabs and return to me.",
                                "Close");
                            return;
                        case < 50:
                            Subject.Reply(source,
                                "The turtles here tend to be soft, I know you'll have no issues with these. Kill 10 Turtles in the sewer.",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerTurtle);
                            return;
                        case < 75:
                            Subject.Reply(source,
                                "Frogs in this sewer are poisonous, when we try to maintain the sewer they are always after you. Will you go kill 10 for me?",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerFrog);
                            return;
                        default:
                            Subject.Reply(source,
                                "Anemones are destructive in the sewers. I need 10 of the Anemones slain please.",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerAnemone);
                            break;
                    }
                }
                
                if (source.UserStatSheet.Level is >= 50 and <= 70)
                {
                    switch (roll)
                    {
                        case < 25:
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerBrawlfish);
                            Subject.Reply(source, "Brawlfish are territorial creatures, difficult to get rid of. Can you slay 10 of them for me?", "Close");
                            return;
                        case < 50:
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerRockCobbler);
                            Subject.Reply(source,
                                "Rock Cobblers usually stay to themselves but lately they've been rather aggressive so they must go. Can you handle 10 of the Rock Cobblers?",
                                "Close");
                            return;
                        case < 75:
                            Subject.Reply(source,
                                "It's an odd place for a Kraken to live, however they are quite comfortable down there. A little too comfortable... Go slay 10 Krakens.",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerKraken);
                            return;
                    }

                    Subject.Reply(source,
                        "The Mini Skrulls are a bit scary, they've made home about half way down. Can you kill five of them?",
                        "Close");
                    source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerMiniSkrull);
                    return;
                }

                if (source.UserStatSheet.Level is >= 71 and <= 99)
                {
                    switch (roll)
                    {
                        case < 33:
                            Subject.Reply(source,
                                "Gogs in the sewer... why? I don't know, but they got to go. Get down there and clear out 10 Gogs for me.",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerGog);
                            return;
                        case < 66:
                            Subject.Reply(source,
                                "Gremlins are tricky little buggers. They've made their home here for quite some time, it's getting out of hand. Can you get rid of 10 Gremlins?",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerGremlin);
                            return;
                        default:
                            Subject.Reply(source,
                                "The Skrull has made his home at the bottom of the Sewer. He can be a pain to defeat, but could you handle him three times for me? I'd recommend bringing a group.",
                                "Close");
                            source.Trackers.Enums.Set(PietSewerKillQuestStage.SewerSkrull);
                            break;
                    }
                }

                break;
            }
        }
    }
}