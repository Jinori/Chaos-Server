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

public class HelpSableScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HelpSableScript(
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
            case "savagesable_initial":
            {
                if (source.StatSheet.Level < 11)
                    return;

                if (source.Trackers.Enums.HasValue(HelpSable.FinishedSable)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedSam)
                    || source.Trackers.Enums.HasValue(HelpSable.FinishedSam)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedRoger)
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
                
                var option = new DialogOption
                {
                    DialogKey = "helpsable_initial",
                    OptionText = "Are you okay?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;
                
                case "helpsable_initial":
            {
                var hasStage = source.Trackers.Enums.TryGetValue(out HelpSable _);

                if (source.Trackers.Enums.HasValue(HelpSable.None) || !hasStage)
                {
                    Subject.Reply(source, "Skip", "HelpSable_initial2");
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.StartedSable))
                {
                    Subject.Reply(source, "Skip", "helpsable_return");
                }
            }
                break;

            case "helpsable_initial4":
            {
               source.Trackers.Enums.Set(HelpSable.StartedSable);
               break;
            }

            case "helpsable_return":
            {
                if (!source.Inventory.HasCount("Red Pirate Bandana", 1))
                {
                    Subject.Reply(source, "I thought we discussed you bringing me a Red Pirate Bandana? Well? Where is it? It doesn't matter now, enjoy the brig!");
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
                
                source.Trackers.Enums.Set(HelpSable.FinishedSable);
                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 75000);
                source.SendOrangeBarMessage("Savage Sable appreciates his Red Pirate Bandana.");
                
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp and Blue Cloak from Helping Sable",
                        source.Name,
                        75000);
            }

                break;
        }
    }
}