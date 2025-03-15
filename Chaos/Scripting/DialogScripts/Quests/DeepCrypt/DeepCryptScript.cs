using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.DeepCrypt;

public class DeepCryptScript(
    Dialog subject,
    ILogger<DeepCryptScript> logger,
    ISimpleCache simpleCache,
    IItemFactory itemFactory) : DialogScriptBase(subject)
{

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "nyxra_initial":
            {
                if (source.UserStatSheet.Level >= 80)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "deepcrypt_initial",
                        OptionText = "Deep Crypt"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                break;
            }

            case "deepcrypt_initial":
            {
                if (source.UserStatSheet.Master && source.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "deepcrypthard_initial",
                        OptionText = "Deep Crypt (Hard)"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                
                if (source.UserStatSheet is { Level: >= 99, Master: true })
                {
                    var option = new DialogOption
                    {
                        DialogKey = "deepcryptmedium_initial",
                        OptionText = "Deep Crypt (Medium)"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                
                if (source.UserStatSheet.Level >= 80)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "deepcrypteasy_initial",
                        OptionText = "Deep Crypt (Easy)"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "deepcrypteasy_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("deepcryptcd", out var cdtime2))
                {
                    Subject.Reply(
                        source,
                        $"You've already attempted this dungeon recently. You can try again in {cdtime2.Remaining.ToReadableString()}.");

                    return;
                }
                
                var option = new DialogOption
                {
                    DialogKey = "deepcrypteasy_start1",
                    OptionText = "I'd like to try easy."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "deepcrypteasy_start2":
            {
                if ((source.Group == null) && !source.IsGodModeEnabled())
                {
                    Subject.Reply(source, "You best bring a group into the Deep Crypt.");

                    return;
                }

                if ((source.Group != null) && !source.IsGodModeEnabled())
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    foreach (var member in group)
                    {
                        if (member.UserStatSheet.Level < 80)
                            Subject.Reply(source, "One of your group members are under level 80.");

                        if (member.Trackers.TimedEvents.HasActiveEvent("deepcryptcd", out _))
                            Subject.Reply(source, "One of your group members have done this too recently.");

                        if (!member.WithinLevelRange(source))
                            Subject.Reply(source, "One of your group members are not in your level range.");

                        if (!member.WithinRange(source))
                            Subject.Reply(source, "You are missing one of your group members.");
                    }
                }

                break;
            }

            case "deepcrypteasy_start3":
            {
                if (source.Group != null)
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    var mapinstance = simpleCache.Get<MapInstance>("deep_crypt1");

                    var rectangle = new Rectangle(
                        5,
                        190,
                        4,
                        4);

                    foreach (var member in group)
                    {
                        var dialog = member.ActiveDialog.Get();
                        dialog?.Close(member);
                        var point = rectangle.GetRandomPoint();
                        member.SendOrangeBarMessage("Clear the Deep Crypt.");
                        member.Trackers.TimedEvents.AddEvent("deepcryptcd", TimeSpan.FromHours(22), true);
                        member.TraverseMap(mapinstance, point);
                    }
                }

                break;
            }
            
            case "deepcryptmedium_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("deepcryptcd", out var cdtime2))
                {
                    Subject.Reply(
                        source,
                        $"You've already attempted this dungeon recently. You can try again in {cdtime2.Remaining.ToReadableString()}.");

                    return;
                }
                
                var option = new DialogOption
                {
                    DialogKey = "deepcryptmedium_start1",
                    OptionText = "I'd like to try medium."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "deepcryptmedium_start2":
            {
                if ((source.Group == null) && !source.IsGodModeEnabled())
                {
                    Subject.Reply(source, "You best bring a group into the Deep Crypt.");

                    return;
                }

                if ((source.Group != null) && !source.IsGodModeEnabled())
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    foreach (var member in group)
                    {
                        if (!member.UserStatSheet.Master)
                            Subject.Reply(source, "One of your group members are not mastered.");

                        if (member.Trackers.TimedEvents.HasActiveEvent("deepcryptcd", out _))
                            Subject.Reply(source, "One of your group members have done this too recently.");

                        if (!member.WithinLevelRange(source))
                            Subject.Reply(source, "One of your group members are not in your level range.");

                        if (!member.WithinRange(source))
                            Subject.Reply(source, "You are missing one of your group members.");
                    }
                }

                break;
            }

            case "deepcryptmedium_start3":
            {
                if (source.Group != null)
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    var mapinstance = simpleCache.Get<MapInstance>("deep_crypt2");

                    var rectangle = new Rectangle(
                        5,
                        190,
                        4,
                        4);

                    foreach (var member in group)
                    {
                        var dialog = member.ActiveDialog.Get();
                        dialog?.Close(member);
                        var point = rectangle.GetRandomPoint();
                        member.SendOrangeBarMessage("Clear the Deep Crypt.");
                        member.Trackers.TimedEvents.AddEvent("deepcryptcd", TimeSpan.FromHours(22), true);
                        member.TraverseMap(mapinstance, point);
                    }
                }

                break;
            }
            
            case "deepcrypthard_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("deepcryptcd", out var cdtime2))
                {
                    Subject.Reply(
                        source,
                        $"You've already attempted this dungeon recently. You can try again in {cdtime2.Remaining.ToReadableString()}.");

                    return;
                }
                
                var option = new DialogOption
                {
                    DialogKey = "deepcrypthard_start1",
                    OptionText = "I'd like to try Hard."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "deepcrypthard_start2":
            {
                if ((source.Group == null) && !source.IsGodModeEnabled())
                {
                    Subject.Reply(source, "You best bring a group into the Deep Crypt.");

                    return;
                }

                if ((source.Group != null) && !source.IsGodModeEnabled())
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    foreach (var member in group)
                    {
                        if (!member.Trackers.Flags.HasFlag(ClassStatBracket.Grandmaster))
                            Subject.Reply(source, "One of your group members are not grandmaster.");

                        if (member.Trackers.TimedEvents.HasActiveEvent("deepcryptcd", out _))
                            Subject.Reply(source, "One of your group members have done this too recently.");

                        if (!member.WithinLevelRange(source))
                            Subject.Reply(source, "One of your group members are not in your level range.");

                        if (!member.WithinRange(source))
                            Subject.Reply(source, "You are missing one of your group members.");
                    }
                }

                break;
            }

            case "deepcrypthard_start3":
            {
                if (source.Group != null)
                {
                    var group = source.Group.ThatAreWithinRange(source);

                    var mapinstance = simpleCache.Get<MapInstance>("deep_crypt3");

                    var rectangle = new Rectangle(
                        5,
                        190,
                        4,
                        4);

                    foreach (var member in group)
                    {
                        var dialog = member.ActiveDialog.Get();
                        dialog?.Close(member);
                        var point = rectangle.GetRandomPoint();
                        member.SendOrangeBarMessage("Clear the Deep Crypt.");
                        member.Trackers.TimedEvents.AddEvent("deepcryptcd", TimeSpan.FromHours(22), true);
                        member.TraverseMap(mapinstance, point);
                    }
                }

                break;
            }
        }
    }
}