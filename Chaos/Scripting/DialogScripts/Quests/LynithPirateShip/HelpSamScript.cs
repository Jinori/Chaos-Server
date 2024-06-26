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

public class HelpSamScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HelpSamScript(
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
            case "saltysam_initial":
            {
                if (source.StatSheet.Level < 41)
                    return;

                var hasStage = source.Trackers.Enums.TryGetValue(out HelpSable stage);

                if (!hasStage
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedSam)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedRoger)
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedRoger)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedCaptain)
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo)
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooFailed)
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart)
                    || source.Trackers.Enums.HasValue(HelpSable.CompletedEscort)
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedDoltoo))
                    return;
                
                if (source.Trackers.Enums.HasValue(HelpSable.StartedSam))
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "helpsam_initial",
                        OptionText = "I got your Black Pirate Bandana."
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(0, option1);
                    
                    return;
                }
                
                
                var option = new DialogOption
                {
                    DialogKey = "helpsam_initial",
                    OptionText = "Are you looking for something?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;
                
                case "helpsam_initial":
            {
                if (source.Trackers.Enums.HasValue(HelpSable.FinishedSable))
                {
                    Subject.Reply(source, "Skip", "HelpSam_initial2");
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.StartedSam))
                {
                    Subject.Reply(source, "Skip", "helpsam_return");
                }
            }
                break;

            case "helpsam_initial4":
            {
               source.Trackers.Enums.Set(HelpSable.StartedSam);
               break;
            }

            case "helpsam_return":
            {
                if (!source.Inventory.HasCount("Black Pirate Bandana", 1))
                {
                    Subject.Reply(source, "Are you hiding the Black Pirate Bandana? That's not nice. I'm going to throw you in the brig!");
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

                source.Inventory.RemoveQuantity("Black Pirate Bandana", 1);
                source.Trackers.Enums.Set(HelpSable.FinishedSam);
                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 250000);
                source.SendOrangeBarMessage("Salty Sam thanks you for his Black Pirate Bandana.");
                
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp from Helping Sam",
                        source.Name,
                        250000);
            }

                break;
        }
    }
}