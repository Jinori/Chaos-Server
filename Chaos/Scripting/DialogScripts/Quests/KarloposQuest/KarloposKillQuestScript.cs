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

namespace Chaos.Scripting.DialogScripts.Quests.KarloposQuest;

public class KarloposKillQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out KarloposKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);
        
        if (tenPercent > 320000)
        {
            tenPercent = 320000;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "kregg_initial":
            {
                if (source.UserStatSheet.Level is >= 41 and < 71)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "karloposkillquest_initial",
                        OptionText = "Karlopos Slayer"
                    };
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "karloposkillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("karloposslayercd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"That pleases me. Please come back to me soon. (({cdtime.Remaining.ToReadableString()}))",
                        "kregg_initial");
                    return;
                }

                if (source.UserStatSheet.Level >= 71)
                {
                    source.Trackers.Counters.Remove("karloposcrabcounter", out _);
                    source.Trackers.Counters.Remove("karloposturtlecounter", out _);
                    source.Trackers.Counters.Remove("karloposslugcounter", out _);
                    source.Trackers.Counters.Remove("karloposoctopuscounter", out _);
                    source.Trackers.Counters.Remove("karloposkrakencounter", out _);
                    source.Trackers.Counters.Remove("karlopossporecounter", out _);
                    source.Trackers.Counters.Remove("karloposgogcounter", out _);
                    Subject.Reply(source, "These sand beast are not a match for you.");
                    return;
                }

                if (!hasStage || stage == KarloposKillQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "karloposkillquest_start",
                        OptionText = "What needs to be done?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                    return;
                }

                if (hasStage && stage == KarloposKillQuestStage.KarloposCrab)
                {
                    if (!source.Trackers.Counters.TryGetValue("karloposcrabcounter", out int crab) || crab < 10)
                    {
                        Subject.Reply(source, "All you need to do is kill 10 crabs.", "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Nice Job! Come back soon.",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karloposcrabcounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == KarloposKillQuestStage.KarloposTurtle)
                {
                    if (!source.Trackers.Counters.TryGetValue("karloposturtlecounter", out int turtle) || turtle < 10)
                    {
                        Subject.Reply(source, "The turtles may be a little hard to kill, but you can do it. Only need 10 of them terminated.", "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That will be enough. Thank you Aisling.",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karloposturtlecounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == KarloposKillQuestStage.KarloposSlug)
                {
                    if (!source.Trackers.Counters.TryGetValue("karloposslugcounter", out int slug) || slug < 10)
                    {
                        Subject.Reply(source, "The slugs are gross, they need to go. Please slay 10 of them!", "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Great! They're such a horrid sight, I'm glad they are gone.",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karloposslugcounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == KarloposKillQuestStage.KarloposSpore)
                {
                    if (!source.Trackers.Counters.TryGetValue("karlopossporecounter", out int spore) || spore < 10)
                    {
                        Subject.Reply(source, "Spores can be a challenge but an Aisling like yourself shouldn't be having this much trouble. It's only 10 of them, I usually do it in my sleep!", "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Easy isn't it? It's like you've been doing this your whole life! I bet you could play guitar.",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karlopossporecounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == KarloposKillQuestStage.KarloposKraken)
                {
                    if (!source.Trackers.Counters.TryGetValue("karloposkrakencounter", out int kraken) || kraken < 10)
                    {
                        Subject.Reply(source, "The krakens in there are nuts! You may be weak but it doesn't mean give up! Slaughter 10 Kraken for me please.", "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"I knew you had it in you, that's what I'm talking about.",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karloposkrakencounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == KarloposKillQuestStage.KarloposOctopus)
                {
                    if (!source.Trackers.Counters.TryGetValue("karloposoctopuscounter", out int octopus) || octopus < 10)
                    {
                        Subject.Reply(source, "I know I asked for 10 Octopus, that's 80 legs of meat! I think you can do it.", "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"I'm going to collect all those legs! That'll feed my family for weeks. Thank you Aisling.",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karloposoctopuscounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == KarloposKillQuestStage.KarloposGog)
                {
                    if (!source.Trackers.Counters.TryGetValue("karloposgogcounter", out int gog) || gog < 10)
                    {
                        Subject.Reply(source,
                            "The Gogs are such ugly monsters aren't they? Are you afraid of them? It's only 10 I need killed, can you do it?",
                            "kregg_initial");
                        return;
                    }

                    logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Glad those are gone now, the beaches will be so much prettier to walk!",
                        "kregg_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("karloposslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("karloposgogcounter", out _);
                    source.Trackers.Enums.Set(KarloposKillQuestStage.None);
                }

                break;
            }

            case "karloposkillquest_start":
            {
                var roll = new Random().Next(1, 101);
                
                if (source.UserStatSheet.Level < 60)
                {
                    if (roll < 33)
                    {
                        source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposCrab);
                        Subject.Reply(source,
                            "Go crack open some crabs, 10 of them to be exact!",
                            "Close");
                        return;
                    }

                    if (roll is > 33 and < 66)
                    {
                        Subject.Reply(source,
                            "Breaking the shells of turtles can be difficult, but you'll manage. Go eliminate 10 Turtles for me..",
                            "Close");
                        source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposTurtle);
                        return;
                    }
                    Subject.Reply(source,
                        "Slugs are gross, but I bet you know how to handle them. Kill 10 Slugs for me.",
                        "Close");
                    source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposSlug);
                    return;
                }
                
                if (source.UserStatSheet.Level is >= 60 and <= 70)
                {
                    if (roll < 25)
                    {
                        source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposSpore);
                        Subject.Reply(source, "Spores are everywhere in there, can you kill 10 for me?", "Close");
                        return;
                    }
                    if (roll < 50)
                    {
                        source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposKraken);
                        Subject.Reply(source,
                            "The Krakens are difficult foes, can you go defeat 10 Kraken to make the beaches safe?",
                            "Close");
                        return;
                    }

                    if (roll < 75)
                    {
                        Subject.Reply(source,
                            "I really like Octopus, sad they have to die for me to eat. Could you go kill 10 for me?",
                            "Close");
                        source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposOctopus);
                        return;
                    }
                    Subject.Reply(source,
                        "The Gogs are so ugly, will you be able to clear 10 please.",
                        "Close");
                    source.Trackers.Enums.Set(KarloposKillQuestStage.KarloposGog);
                }
                break;
            }
        }
    }
}