using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
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
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.WestWoodlands;

public class WWDungeonScript(
    Dialog subject,
    ILogger<WWDungeonScript> logger,
    ISimpleCache simpleCache,
    IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } =
        DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out WestWoodlandsDungeonQuestStage stage);

        var gearDictionary = new Dictionary<(BaseClass, Gender), string[]>
        {
            { (BaseClass.Warrior, Gender.Male), ["ipletmail", "iplethelmet"] },
            { (BaseClass.Warrior, Gender.Female), ["labyrinthmail", "labyrinthhelmet"] },
            { (BaseClass.Monk, Gender.Male), ["windgarb"] },
            { (BaseClass.Monk, Gender.Female), ["lightninggarb"] },
            { (BaseClass.Rogue, Gender.Male), ["keaton"] },
            { (BaseClass.Rogue, Gender.Female), ["pebblerose"] },
            { (BaseClass.Priest, Gender.Male), ["hierophant"] },
            { (BaseClass.Priest, Gender.Female), ["dolman"] },
            { (BaseClass.Wizard, Gender.Male), ["mane"] },
            { (BaseClass.Wizard, Gender.Female), ["clymouth"] }
        };

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
                if (source.UserStatSheet.Master || (source.UserStatSheet.Level >= 71))
                {
                    Subject.Reply(source,"Lost Woodlands is beneath you now. Go pick on something your own size.");

                    return;
                }
                
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
                if (source.Group == null && !source.IsGodModeEnabled())
                {
                    Subject.Reply(source, "You best bring a group into the Lost Woods.");
                    return;
                }

                if (source.Group != null && !source.IsGodModeEnabled())
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    foreach (var member in group)
                    {
                        if (member.UserStatSheet.Level < 50)
                        {
                            Subject.Reply(source, "One of your group members are under level 50.");
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
                Subject.Reply(source, "Looks like you weren't able to clear the Lost Woods in time. That's okay.",
                    "wwdungeon_initial");
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

            case "wwdungeon_turnin3":
            {
                source.Trackers.Enums.Set(WestWoodlandsDungeonQuestStage.None);
                source.Trackers.Counters.AddOrIncrement("wwdungeon");
                source.Trackers.TimedEvents.AddEvent("wwdungeoncd", TimeSpan.FromHours(22), true);
                ExperienceDistributionScript.GiveExp(source, 500000);
                source.TryGiveGamePoints(10);

                logger.WithTopics(
                        [Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest])
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation("{@AislingName} has received {@ExpAmount} exp from a Lost Woodlands quest",
                        source.Name,
                        500000);


                source.Trackers.Counters.TryGetValue("wwdungeon", out var count);

                var roll = IntegerRandomizer.RollSingle(101);

                switch (roll)
                {
                    case < 10:
                    {
                        if (!source.Trackers.Flags.HasFlag(AvailableMounts.Ant))
                        {
                            source.Trackers.Flags.AddFlag(AvailableMounts.Ant);
                            source.SendOrangeBarMessage("Maxwell hands you a new mount!");
                            source.TryGiveGamePoints(5);
                        }
                        else
                        {
                            var item = itemFactory.Create("sparklering");
                            source.GiveItemOrSendToBank(item);
                            source.SendOrangeBarMessage("Maxwell hands you a Sparkle Ring!");
                            source.TryGiveGamePoints(5);
                        }

                        break;
                    }

                    case < 25:
                    {
                        var item = itemFactory.Create("ialtagseye");
                        source.GiveItemOrSendToBank(item);
                        source.SendOrangeBarMessage("Maxwell hands you a Ialtag's Eye!");
                        source.TryGiveGamePoints(5);
                        break;
                    }

                    case < 40:
                    {
                        var boots = itemFactory.Create("silkboots");
                        source.GiveItemOrSendToBank(boots);
                        source.SendOrangeBarMessage("Maxwell thanks you with some boots he had.");
                        source.TryGiveGamePoints(5);
                        break;
                    }
                    case < 55:
                    {
                        var ring = itemFactory.Create("sonorring");
                        source.GiveItemOrSendToBank(ring);
                        source.SendOrangeBarMessage("Maxwell hands you a Sonor Ring!");
                        source.TryGiveGamePoints(5);
                        break;
                    }
                    case < 70:
                    {
                        var ring = itemFactory.Create("myanmarring");
                        source.GiveItemOrSendToBank(ring);
                        source.SendOrangeBarMessage("Maxwell thanks you with a spare ring he had.");
                        source.TryGiveGamePoints(5);
                        break;
                    }
                    case < 83:
                    {
                        if (!source.Inventory.ContainsByTemplateKey("cherubwings") || !source.Equipment.ContainsByTemplateKey("cherubwings") ||
                            !source.Bank.Contains("Cherub Wings"))
                        {
                            var wings = itemFactory.Create("cherubwings");
                                                    source.GiveItemOrSendToBank(wings);
                                                    source.SendOrangeBarMessage("Maxwell thanks you with some wings he had.");
                                                    source.TryGiveGamePoints(5);
                                                    return;
                        }

                        source.TryGiveGold(75000);
                        source.SendOrangeBarMessage("Maxwell thanks you with 75,000 Gold.");
                        source.TryGiveGamePoints(5);
                        
                        break;
                    }
                    case < 101:
                    {
                        source.SendOrangeBarMessage("Maxwell just thanks you.");
                        source.TryGiveGamePoints(20);
                        break;
                    }
                }

                break;
            }

            case "wwdungeon_turnin2":
            {
                source.Trackers.Enums.Set(WestWoodlandsDungeonQuestStage.None);
                source.Trackers.Counters.AddOrIncrement("wwdungeon");
                source.Trackers.TimedEvents.AddEvent("wwdungeoncd", TimeSpan.FromHours(22), true);
                ExperienceDistributionScript.GiveExp(source, 500000);
                source.TryGiveGamePoints(10);

                logger.WithTopics(
                        [Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest])
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation("{@AislingName} has received {@ExpAmount} exp from a Lost Woodlands quest",
                        source.Name,
                        500000);


                source.Trackers.Counters.TryGetValue("wwdungeon", out _);

                var gearKey = (source.UserStatSheet.BaseClass, source.Gender);
                if (gearDictionary.TryGetValue(gearKey, out var gear))
                {
                    foreach (var gearItemName in gear)
                    {
                        var gearItem = itemFactory.Create(gearItemName);
                        source.GiveItemOrSendToBank(gearItem);
                        source.SendOrangeBarMessage("Maxwell thanks you with some armor he had.");
                    }
                }
            }

                break;
        }
    }
}