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


namespace Chaos.Scripting.DialogScripts.Quests.Astrid;

public class AstridKillQuestScript : DialogScriptBase
{
    private readonly ILogger<TheSacrificeQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public AstridKillQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger) 
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out AstridKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);
        
        if (tenPercent > 100000)
        {
            tenPercent = 100000;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "phio_initial":
            {
                if (source.UserStatSheet.Level is >= 11 and < 71)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "astridkillquest_initial",
                        OptionText = "Astrid Slayer"
                    };
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "astridkillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("astridslayercd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"Ah, you powerful Aisling! You're back! I knew you could do it, they don't stand a chance. I will need your help again soon. Come back and see me soon. (({cdtime.Remaining.ToReadableString()}))",
                        "phio_initial");
                    return;
                }

                if (!hasStage || stage == AstridKillQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "astridkillquest_start",
                        OptionText = "I'm ready for a fight."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                    return;
                }

                if (hasStage && stage == AstridKillQuestStage.AstridWolf)
                {
                    if (!source.Trackers.Counters.TryGetValue("astridwolfcounter", out int wolf) || wolf < 10)
                    {
                        Subject.Reply(source, "Why are you back so soon? That isn't enough wolves! I need 10 of them dead! Get back in there!", "phio_initial");
                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Wonderful Job Aisling! You're stronger than you look. Thank you so much, please take this. I'm sure this isn't the end of it. Come back soon.",
                        "phio_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("astridslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("astridwolfcounter", out _);
                    source.Trackers.Enums.Set(AstridKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == AstridKillQuestStage.AstridKobold)
                {
                    if (!source.Trackers.Counters.TryGetValue("astridkoboldcounter", out int kobold) || kobold < 10)
                    {
                        Subject.Reply(source, "You thought you can handle them huh? You're back already, so weak. Get back in there and kill 10!", "phio_initial");
                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Wonderful Job Aisling! You're stronger than you look. Thank you so much, please take this. I'm sure this isn't the end of it. Come back soon.",
                        "phio_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("astridslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("astridkoboldcounter", out _);
                    source.Trackers.Enums.Set(AstridKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == AstridKillQuestStage.AstridGoblinGuard)
                {
                    if (!source.Trackers.Counters.TryGetValue("astridgoblinguardcounter", out int goblinguard) || goblinguard < 10)
                    {
                        Subject.Reply(source, "They kicked your ass didn't they? I knew they would. You even look weak. Go try again, remember 10 of the Goblin Guards need to be killed!", "phio_initial");
                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Wonderful Job Aisling! You're stronger than you look. Thank you so much, please take this. I'm sure this isn't the end of it. Come back soon.",
                        "phio_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("astridslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("astridgoblinguardcounter", out _);
                    source.Trackers.Enums.Set(AstridKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == AstridKillQuestStage.AstridGoblinSoldier)
                {
                    if (!source.Trackers.Counters.TryGetValue("astridgoblinsoldiercounter", out int goblinsoldier) || goblinsoldier < 10)
                    {
                        Subject.Reply(source, "Figures, I put the wrong Aisling up to the task. Get lost, come back when you've killed 10 Goblin Soldiers.", "phio_initial");
                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Wonderful Job Aisling! You're stronger than you look. Thank you so much, please take this. I'm sure this isn't the end of it. Come back soon.",
                        "phio_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("astridslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("astridgoblinsoldiercounter", out _);
                    source.Trackers.Enums.Set(AstridKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == AstridKillQuestStage.AstridGoblinWarrior)
                {
                    if (!source.Trackers.Counters.TryGetValue("astridgoblinwarriorcounter", out int goblinwarrior) || goblinwarrior < 10)
                    {
                        Subject.Reply(source, "Really? Back so soon? I thought you had more fight in you, pathetic. Try again, remember 10 of the Goblin Warriors need to be killed.", "phio_initial");
                        return;
                    }

                    Logger.WithTopics(
                            [Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest])
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Wonderful Job Aisling! You're stronger than you look. Thank you so much, please take this. I'm sure this isn't the end of it. Come back soon.",
                        "phio_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("astridslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("astridgoblinwarriorcounter", out _);
                    source.Trackers.Enums.Set(AstridKillQuestStage.None);
                }
                

                break;
            }

            case "astridkillquest_start":
            {
                var roll = new Random().Next(1, 101);
                
                if (source.UserStatSheet.Level < 30)
                {
                    if (roll < 50)
                    {
                        source.Trackers.Enums.Set(AstridKillQuestStage.AstridWolf);
                        Subject.Reply(source,
                            "You're ready to fight huh? Go kill 10 Wolves in there for me. Show me what you're made of.",
                            "Close");
                        return;
                    }
                    Subject.Reply(source,
                        "A fight is what you'll get! Go kill 10 Kobolds for me then.",
                        "Close");
                    source.Trackers.Enums.Set(AstridKillQuestStage.AstridKobold);
                    return;
                }
                
                if (source.UserStatSheet.Level >= 30)
                {
                    if (roll < 30)
                    {
                        source.Trackers.Enums.Set(AstridKillQuestStage.AstridGoblinGuard);
                        Subject.Reply(source,
                            "Heh, you won't be so tough after this. Kill 10 Goblin Guards for me then if you're really ready to fight.",
                            "Close");
                        return;
                    }

                    if (roll < 60)
                    {
                        Subject.Reply(source,
                            "I'm really glad. I could use someone to take care of these beast. Go in there and kill 10 Goblin Warriors.",
                            "Close");
                        source.Trackers.Enums.Set(AstridKillQuestStage.AstridGoblinWarrior);
                        return;
                    }
                    Subject.Reply(source,
                        "Think you can really handle it? Well let's see it then. Go kill 10 Goblin Soldiers.",
                        "Close");
                    source.Trackers.Enums.Set(AstridKillQuestStage.AstridGoblinSoldier);
                }
                break;
            }
        }
    }
}