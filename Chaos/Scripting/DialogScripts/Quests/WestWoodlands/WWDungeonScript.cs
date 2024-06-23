using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.WestWoodlands;

public class WWDungeonScript(Dialog subject, ILogger<WWDungeonScript> logger, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out WestWoodlandsDungeonQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "maxwell_initial":
            {
                if (source.UserStatSheet.Level >= 50)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "wwdungeon_initial",
                        OptionText = "Lost Woodlands"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "wwdungeon_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("wwdungeoncd", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I'm sure glad you cleared those woods. That was awesome. They'll come back quickly, let's do this again tomorrow? (({cdtime.Remaining.ToReadableString()}))",
                        "maxwell_initial");
                    return;
                }

                if (!hasStage || stage == WestWoodlandsDungeonQuestStage.None)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "wwdungeon_start",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Enums.HasValue(WestWoodlandsDungeonQuestStage.Started))
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("wwdungeontimer", out _))
                    {
                        Subject.Reply(source, "Skip", "wwdungeon_return2");
                        return;
                    }

                    Subject.Reply(source, "Skip", "wwdungeon_return");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WestWoodlandsDungeonQuestStage.Completed))
                {
                    Subject.Reply(source, "Skip", "wwdungeon_turnin");
                }

                break;
            }

            case "wwdungeon_start2":
            {
                if (source.Group == null)
                {
                    Subject.Reply(source, "You best bring a group into the Lost Woods.");
                    return;
                }

                if (source.Group != null)
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    foreach (var member in group)
                    {
                        if (member.UserStatSheet.Level < 50)
                        {
                            Subject.Reply(source, "One of your group members are over level 50.");
                        }

                        if (member.Trackers.TimedEvents.HasActiveEvent("wwdungeoncd", out _))
                        {
                            Subject.Reply(source, "One of your group members have done this too recently.");
                        }

                        if (!member.WithinLevelRange(source))
                        {
                            Subject.Reply(source, "One of your group members are not in your level range.");
                        }

                        if (!member.WithinRange(source))
                        {
                            Subject.Reply(source, "You are missing one of your group members.");
                        }
                    }
                }
                break;
            }

            case "wwdungeon_start3":
            {
                if (source.Group != null)
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    var mapinstance = simpleCache.Get<MapInstance>("lost_woodlands");
                    var rectangle = new Rectangle(83, 143, 4, 4);
                    
                    foreach (var member in group)
                    {
                        var dialog = member.ActiveDialog.Get();
                        dialog?.Close(member);
                        member.Trackers.Enums.Set(WestWoodlandsDungeonQuestStage.Started);
                        var point = rectangle.GetRandomPoint();
                        member.SendOrangeBarMessage("Clear the Lost Woodlands of all monsters.");
                        member.Trackers.TimedEvents.AddEvent("wwdungeontimer", TimeSpan.FromHours(2), true);
                        member.TraverseMap(mapinstance, point);
                    }
                }
                break;
            }

            case "wwdungeon_return":
            {
                source.Trackers.Enums.Set(WestWoodlandsDungeonQuestStage.None);
                Subject.Reply(source, "Looks like you weren't able to clear the Lost Woods in time. That's okay.", "wwdungeon_initial");
                break;
            }

            case "wwdungeon_return3":
            {
                var mapinstance = simpleCache.Get<MapInstance>("lost_woodlands");
                var rectangle = new Rectangle(83, 143, 4, 4);
                var point = rectangle.GetRandomPoint();
                source.TraverseMap(mapinstance, point);
                
                break;
            }

            case "wwdungeon_turnin2":
            {
                source.Trackers.Enums.Set(WestWoodlandsDungeonQuestStage.None);
                source.Trackers.TimedEvents.AddEvent("wwdungeoncd", TimeSpan.FromHours(22), true);
                ExperienceDistributionScript.GiveExp(source, 500000);
                source.TryGiveGamePoints(10);
                source.SendOrangeBarMessage("Maxwell thanks you for all your efforts.");
                
                logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation("{@AislingName} has received {@ExpAmount} exp from a Lost Woodlands quest", source.Name,
                        500000);
                
                break;
            }
        }
    }
}