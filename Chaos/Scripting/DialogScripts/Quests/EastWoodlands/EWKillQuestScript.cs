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

namespace Chaos.Scripting.DialogScripts.Quests.EastWoodlands;

public class EWKillQuestScript : DialogScriptBase
{
    private readonly ILogger<theSacrificeQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public EWKillQuestScript(Dialog subject, ILogger<theSacrificeQuestScript> logger) 
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out EastWoodlandsKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ghislain_initial":
            {
                if (source.UserStatSheet.Level < 41)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "ewkillquest_initial",
                        OptionText = "Eastwoodlands Slayer"
                    };
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "ewkillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("ewslayercd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"You've done exceptionally well Aisling. Please come back to me soon. (({cdtime.Remaining.ToReadableString()}))",
                        "ghislain_initial");
                    return;
                }

                if (source.UserStatSheet.Level >= 41)
                {
                    Subject.Reply(source, "You are too strong to be worried about these pest.");
                    return;
                }

                if (!hasStage || stage == EastWoodlandsKillQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "ewkillquest_start",
                        OptionText = "What do you need me to do?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                    return;
                }

                if (hasStage && stage == EastWoodlandsKillQuestStage.EWViper)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewvipercounter", out int viper) || viper < 10)
                    {
                        Subject.Reply(source, "You couldn't find 10 vipers to kill?", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"Wonderful Job Aisling! You're stronger than you look. Thank you so much, please take this. I'm sure this isn't the end of it. Come back soon.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewvipercounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWBee1)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewbeecounter1", out int ewbee1) || ewbee1 < 10)
                    {
                        Subject.Reply(source, "The bees in the enchanted garden aren't so bad but they're over populated. Go clear 10 of them.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewbeecounter1", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWBee2)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewbeecounter2", out int ewbee2) || ewbee2 < 10)
                    {
                        Subject.Reply(source, "I know those bees can be painful with their stingers but it's important, they must die.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewbeecounter2", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWMantis1)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewmantiscounter1", out int ewmantis1) || ewmantis1 < 10)
                    {
                        Subject.Reply(source, "The mantis in the Enchanted Garden can be very sneaky, they blend so well with the grass. I understand you are struggling, but you can do this. Go slay 10 Mantis.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewmantiscounter1", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWMantis2)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewmantiscounter2", out int ewmantis2) || ewmantis2 < 10)
                    {
                        Subject.Reply(source, "The mantis are dangerous foes but don't fear. I have faith in you. Kill 10 of the mantis.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewmantiscounter2", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWWolf)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewwolfcounter", out int ewwolf) || ewwolf < 10)
                    {
                        Subject.Reply(source, "Heh, having trouble with the wolves? I can sense your fear, so they can too. Overcome it, slay 10 wolves.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewwolfcounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWKobold)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewkoboldcounter", out int ewkobold) || ewkobold < 10)
                    {
                        Subject.Reply(source, "Those verticle dogs are hardly standing. Go knock them over and kill them with ease. Slay 10 Kobolds.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewkoboldcounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWKoboldMage)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewkoboldmagecounter", out int ewkoboldmage) || ewkoboldmage < 10)
                    {
                        Subject.Reply(source, "Those vertical dogs with staves are impressive. But you are even more incredible Aisling, go teach them a lesson. Slay 10 Kobold Mages.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewkoboldmagecounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWGoblinSoldier)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewgoblinsoldiercounter", out int ewgoblinsoldier) || ewgoblinsoldier < 10)
                    {
                        Subject.Reply(source, "Goblin Soldiers can be tough but they're only following orders. There's no reasoning with them, slay 10 goblin soldiers.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewgoblinsoldiercounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWGoblinWarrior)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewgoblinwarriorcounter", out int ewgoblinwarrior) || ewgoblinwarrior < 10)
                    {
                        Subject.Reply(source, "Goblin Warriors are loyal to their brethren and cannot be reasoned with. Please slay 10 Goblin Warriors.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewgoblinwarriorcounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWGoblinGuard)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewgoblinguardcounter", out int ewgoblinguard) || ewgoblinguard < 10)
                    {
                        Subject.Reply(source, "Goblin Guards sole purpose is to defend their chiefs. They will stop at nothing to keep their chiefs safe. You must slay 10 Goblin Guards.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewgoblinguardcounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                    return;
                }
                if (hasStage && stage == EastWoodlandsKillQuestStage.EWHobgoblin)
                {
                    if (!source.Trackers.Counters.TryGetValue("ewhobgoblincounter", out int ewhobgoblin) || ewhobgoblin < 10)
                    {
                        Subject.Reply(source, "Hobgoblins are incredibly different from normal goblins. They don't care for orders or authority. That makes them even more dangerous. Please go kill 10 Hobgoblins.", "ghislain_initial");
                        return;
                    }

                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name,
                            tenPercent);

                    Subject.Reply(source,
                        $"That's plenty. Good work! I saw your strength and knew you'd get the job done.",
                        "ghislain_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("ewslayercd", TimeSpan.FromHours(8), true);
                    source.Trackers.Counters.Remove("ewhobgoblincounter", out _);
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.None);
                }

                break;
            }

            case "ewkillquest_start":
            {
                var roll = new Random().Next(1, 101);
                
                if (source.UserStatSheet.Level <= 11)
                {
                    if (roll < 33)
                    {
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWViper);
                        Subject.Reply(source,
                            "Take down 10 Vipers from the Enchanted Garden.",
                            "Close");
                        return;
                    }

                    if (roll is > 33 and < 66)
                    {
                        Subject.Reply(source,
                            "Go slay 10 bees in the enchanted garden, watch for their stingers they pack quite a punch.",
                            "Close");
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWBee1);
                        return;
                    }
                    Subject.Reply(source,
                        "Hunt the mantis like they hunt you, kill 10 of those sneaky pest in the enchanted garden.",
                        "Close");
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWMantis1);
                    return;
                }
                
                if (source.UserStatSheet.Level is >= 12 and <= 25)
                {
                    if (roll < 25)
                    {
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWMantis2);
                        Subject.Reply(source, "Those mantis in the forest have been catching Aislings off guard. Go handle 10 of them for me.", "Close");
                        return;
                    }
                    if (roll < 50)
                    {
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWWolf);
                        Subject.Reply(source,
                            "The wolves are no match for you Aisling, go slay 10 of them quickly.",
                            "Close");
                        return;
                    }

                    if (roll < 75)
                    {
                        Subject.Reply(source,
                            "Let's see how well you do against the Kobolds. They shouldn't give you any issues, take down 10 of the Kobolds.",
                            "Close");
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWKobold);
                        return;
                    }
                    Subject.Reply(source,
                        "The bees aren't too bad to fight, just watch out for their stingers. Kill 10 of the bees in the forest.",
                        "Close");
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWBee2);
                    return;
                }
                if (source.UserStatSheet.Level is >= 26 and <= 36)
                {
                    if (roll < 25)
                    {
                       Subject.Reply(source,
                                               "You need to pay close attention to the Kobold Mages, they are no joke. I think you can handle it though if you use your brains. Go take out 10 Kobold Mages.",
                                               "Close");
                                           source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWKoboldMage);
                                           return; 
                    }
                    
                    if (roll < 50)
                    {
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWGoblinSoldier);
                        Subject.Reply(source,
                            "These goblin soldiers should be no problems for you Aisling. Go take down 10 of them.",
                            "Close");
                        return;
                    }

                    if (roll < 75)
                    {
                        Subject.Reply(source,
                            "The goblin warriors are tough, let me see you take down 10 of them. I know you can do it.",
                            "Close");
                        source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWGoblinWarrior);
                        return;
                    }
                    Subject.Reply(source,
                        "Those guards only care about protecting their chief. Makes them quite dangerous. I wouldn't put this on you if I didn't think you can handle it. Kill 10 of the Goblin Guards.",
                        "Close");
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWGoblinGuard);
                }

                if (source.UserStatSheet.Level > 36)
                {
                    source.Trackers.Enums.Set(EastWoodlandsKillQuestStage.EWHobgoblin);
                    Subject.Reply(source, "The hobgoblins are the only thing left for you to take down. Please go defeat 10 of them.", "Close");
                }
                break;
            }
        }
    }
}