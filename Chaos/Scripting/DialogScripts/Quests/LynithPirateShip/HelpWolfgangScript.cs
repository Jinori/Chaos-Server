using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.PFQuest;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.LynithPirateShip;

public class HelpWolfgangScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HelpWolfgangScript(
        Dialog subject,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger
    )
        : base(subject)
    {
        SimpleCache = simpleCache;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "captainwolfgang_initial":
            {
                if (source.StatSheet.Level < 99)
                    return;
                var hasStage = source.Trackers.Enums.TryGetValue(out HelpSable stage);

                if (!hasStage 
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo)
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooFailed)
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart)
                    || source.Trackers.Enums.HasValue(HelpSable.CompletedEscort)
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedDoltoo)
                   )
                    return;
                
                if (source.Trackers.Enums.HasValue(HelpSable.StartedCaptain))
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "helpwolfgang_initial",
                        OptionText = "Captain, I got your Male Pirate Garb."
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(0, option1);
                    
                    return;
                }
                
                var option = new DialogOption
                {
                    DialogKey = "helpwolfgang_initial",
                    OptionText = "What else do you need Captain?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;
                
                case "helpwolfgang_initial":
            {
                if (source.Trackers.Enums.HasValue(HelpSable.FinishedRoger))
                {
                    Subject.Reply(source, "Skip", "helpwolfgang_initial2");
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.StartedCaptain))
                {
                    Subject.Reply(source, "Skip", "helpwolfgang_return");
                }
            }
                break;

            case "helpwolfgang_initial4":
            {
               source.Trackers.Enums.Set(HelpSable.StartedCaptain);
               break;
            }

            case "helpwolfgang_return":
            {
                if (!source.Inventory.HasCount("M Pirate Garb", 1))
                {
                    Subject.Reply(source, "Is this a joke? I don't have time for nonsense! Off to the brig!");
                    var point = new Point(30, 17);

                    switch (source.UserStatSheet.Level)
                    {
                        case >= 11 and < 25:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig1");
                            source.TraverseMap(mapinstance1, point);
                            return;
                        }
                        case >= 25 and < 41:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig2");
                            source.TraverseMap(mapinstance1, point);
                            return;
                        }
                        case >= 41 and < 55:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig3");
                            source.TraverseMap(mapinstance1, point);
                            return;
                        }
                        case >= 55 and < 71:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig4");
                            source.TraverseMap(mapinstance1, point);
                            return;
                        }
                        case >= 71 and < 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig5");
                            source.TraverseMap(mapinstance1, point);
                            return;
                        }
                        case >= 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig6");
                            source.TraverseMap(mapinstance1, point);
                            return;
                        }
                    }
                    return;
                }

                source.Inventory.RemoveQuantity("M Pirate Shirt", 1);
                source.Trackers.Enums.Set(HelpSable.FinishedCaptain);
                source.TryGiveGamePoints(10);
                ExperienceDistributionScript.GiveExp(source, 1500000);
                source.SendOrangeBarMessage("Captain Wolfgang seems more outgoing now.");
                
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp from Helping Captain Wolfgang.",
                        source.Name,
                        1500000);
            }
                break;
        }
    }
}