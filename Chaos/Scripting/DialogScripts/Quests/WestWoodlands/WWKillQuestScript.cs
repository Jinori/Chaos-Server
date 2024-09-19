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

namespace Chaos.Scripting.DialogScripts.Quests.WestWoodlands;

public class WWKillQuestScript(Dialog subject, ILogger<WWKillQuestScript> logger) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out WestWoodlandsKillQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);
        
        if (tenPercent > 320000)
        {
            tenPercent = 320000;
        }
        
        if (source.StatSheet.Level >= 99)
        {
            tenPercent = 10000000;
        }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "maxwell_initial":
            {
                if (source.UserStatSheet.Level >= 41)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "wwkillquest_initial",
                        OptionText = "Westwoodlands Slayer"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "wwkillquest_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("wwslayercd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"You've done exceptionally well Aisling. Please come back to me soon. (({cdtime.Remaining.ToReadableString()}))",
                        "maxwell_initial");
                    return;
                }

                if (source.UserStatSheet.Level >= 71)
                {
                    source.Trackers.Counters.Remove("wwgoblinguardcounter", out _);
                    source.Trackers.Counters.Remove("wwgoblinwarriorcounter", out _);
                    source.Trackers.Counters.Remove("wwhobgoblincounter", out _);
                    source.Trackers.Counters.Remove("wwshriekercounter", out _);
                    source.Trackers.Counters.Remove("wwfaeriecounter", out _);
                    source.Trackers.Counters.Remove("wwwispcounter", out _);
                    source.Trackers.Counters.Remove("wwbosscounter", out _);
                    Subject.Reply(source,
                        "This place is now beneath you Aisling, go forth and defeat stronger creatures.");
                    return;
                }

                if (!hasStage || stage == WestWoodlandsKillQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "wwkillquest_start",
                        OptionText = "What do you need me to do?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWGoblinGuard)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwgoblinguardcounter", out int goblinguard) ||
                        goblinguard < 10)
                    {
                        Subject.Reply(source, "No? Couldn't kill 10 Goblin Guards? Try again.", "maxwell_initial");
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
                        $"Excellent! Those goblin guards won't know what hit them. Here take this, good work. I'll need more help soon.",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwgoblinguardcounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWGoblinWarrior)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwgoblinwarriorcounter", out int wwgoblinwarrior) ||
                        wwgoblinwarrior < 10)
                    {
                        Subject.Reply(source, "I wonder if you can handle 10 Goblin Warriors... maybe it was too hard?",
                            "maxwell_initial");
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
                        $"I knew you'd demolish that challenge, great! Come back and see me soon, oh here take this.",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwgoblinwarriorcounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWHobGoblin)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwhobgoblincounter", out int hobgoblin) ||
                        hobgoblin < 10)
                    {
                        Subject.Reply(source,
                            "Those Hobgoblins can be tough. I know you'll get through their armor, keep trying.",
                            "maxwell_initial");
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
                        $"I saw you fight that Hobgoblin, it was incredible. I knew I could rely on you. Take this and come back soon!",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwhobgoblincounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWShrieker)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwshriekercounter", out int shrieker) || shrieker < 10)
                    {
                        Subject.Reply(source,
                            "Those dang Shriekers, terrible little fungi. Please, you have to take out 10 of them.",
                            "maxwell_initial");
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
                        $"Perfect. Those Shriekers won't bother anyone else. Come back and see me tomorrow, I'll have more work for you.",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwshriekercounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWWisp)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwwispcounter", out int wisp) || wisp < 10)
                    {
                        Subject.Reply(source,
                            "Those Wisp are pretty powerful huh, I kind of suspected you to have issues. Give it another go.",
                            "maxwell_initial");
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
                        $"It's nice not seeing all those Wisp in there anymore, well done. Take this for your troubles and come see me again soon. ",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwwispcounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWFaerie)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwfaeriecounter", out int faerie) || faerie < 10)
                    {
                        Subject.Reply(source,
                            "Those faeries are pretty quick, I knew you'd struggle but that's ok. Give it another try!",
                            "maxwell_initial");
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
                        $"Faeries down! Great work, bet you got some wings aye? Those are always useful. Here's your reward, come back and see me soon.",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwfaeriecounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                if (hasStage && stage == WestWoodlandsKillQuestStage.WWTwink)
                {
                    if (!source.Trackers.Counters.TryGetValue("wwbosscounter", out int twink) || twink < 3)
                    {
                        Subject.Reply(source,
                            "Is the Twink giving you troubles? He isn't weak, good luck! Kill him three times!",
                            "maxwell_initial");
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
                        $"Wow, I'm impressed, you took him down three times. Here's your reward, I'll have another task for you tomorrow.",
                        "maxwell_initial");
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, tenPercent);
                    source.Trackers.TimedEvents.AddEvent("wwslayercd", TimeSpan.FromHours(22), true);
                    source.Trackers.Counters.Remove("wwbosscounter", out _);
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.None);
                    return;
                }

                break;
            }

            case "wwkillquest_start":
            {
                var roll = new Random().Next(1, 101);

                if (source.UserStatSheet.Level <= 55)
                {
                    if (roll < 33)
                    {
                        source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWGoblinGuard);
                        Subject.Reply(source,
                            "There's some strong Goblin Guards in there, take down 10 of them.",
                            "Close");
                        return;
                    }

                    if (roll is > 33 and < 66)
                    {
                        Subject.Reply(source,
                            "Please go take care of those Goblin Warriors, they're harrassing the Aislings traveling these woods. Kill off 10 of them.",
                            "Close");
                        source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWGoblinWarrior);
                        return;
                    }

                    Subject.Reply(source,
                        "Hobgoblins are skilled hunters in these woods. It's dangerous in there with them lurking, say could you clear 10 of them for me?",
                        "Close");
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWHobGoblin);
                    return;
                }

                if (source.UserStatSheet.Level is >= 55 and <= 70)
                {
                    if (roll < 25)
                    {
                        source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWShrieker);
                        Subject.Reply(source,
                            "Those Shriekers are deadly, someone should really knock them down. Oh, you should knock them down! Please kill 10 Shriekers.",
                            "Close");
                        return;
                    }

                    if (roll < 50)
                    {
                        source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWWisp);
                        Subject.Reply(source,
                            "Those Wisp are aggressive! Almost took me out and I wasn't even near it... Could you clear the path of 10 Wisp?",
                            "Close");
                        return;
                    }

                    if (roll < 75)
                    {
                        Subject.Reply(source,
                            "The Faeries here are beautiful, but very scary. I'd advise you don't mess with them, but I would like to see 10 killed... so will you take out 10 Faeries?",
                            "Close");
                        source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWFaerie);
                        return;
                    }

                    Subject.Reply(source,
                        "There's a special foe lurking these woods, Twink is by far the dangerous thing here and he doesn't take kindly to visitors. Show him we won't back down, defeat the Twink three times.",
                        "Close");
                    source.Trackers.Enums.Set(WestWoodlandsKillQuestStage.WWTwink);
                    return;
                }

                break;
            }
        }
    }
}