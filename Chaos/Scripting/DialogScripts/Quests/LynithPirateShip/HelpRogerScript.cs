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

public class HelpRogerScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HelpRogerScript(
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
            case "jollyroger_initial":
            {
                if (source.StatSheet.Level < 71)
                    return;
                var hasStage = source.Trackers.Enums.TryGetValue(out HelpSable stage);

                if (!hasStage
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedRoger)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedCaptain)
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain) 
                    || source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooFailed) 
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart) 
                    || source.Trackers.Enums.HasValue(HelpSable.CompletedEscort) 
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedDoltoo)
                   )
                    return;
                
                if (source.Trackers.Enums.HasValue(HelpSable.StartedRoger))
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "helproger_initial",
                        OptionText = "I found a Male Pirate Shirt."
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(0, option1);
                    
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.FinishedSam))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "helproger_initial",
                        OptionText = "Could I get you something?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }
                break;
                
                case "helproger_initial":
            {
                if (source.Trackers.Enums.HasValue(HelpSable.FinishedSam))
                {
                    Subject.Reply(source, "Skip", "helproger_initial2");
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.StartedRoger))
                {
                    Subject.Reply(source, "Skip", "helproger_return");
                }
            }
                break;

            case "helproger_initial3":
            {
               source.Trackers.Enums.Set(HelpSable.StartedRoger);
               break;
            }

            case "helproger_return":
            {
                if (!source.Inventory.HasCount("M Pirate Shirt", 1))
                {
                    Subject.Reply(source, "I don't see this shirt you promised, don't come back without it. You are going in the brig!");
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
                source.Trackers.Enums.Set(HelpSable.FinishedRoger);
                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 750000);
                source.SendOrangeBarMessage("Jolly Roger sees the good in you.");
                
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp from Helping Roger",
                        source.Name,
                        750000);
            }

                break;
        }
    }
}