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
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.LynithPirateShip;

public class HelpDoltooScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HelpDoltooScript(
        Dialog subject,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger, IItemFactory itemFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Logger = logger;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "doltoo_initial":
            {
                if (!source.UserStatSheet.Master)
                    return;

                if (!source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain) 
                    && !source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    && !source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart) 
                    && !source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooFailed))
                    return;
                
                var hasStage = source.Trackers.Enums.TryGetValue(out HelpSable stage);

                if (!hasStage)
                    return;

                if (source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart))
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "helpdoltoo_initial",
                        OptionText = "Sorry I left you."
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(0, option1);
                    
                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "helpdoltoo_initial",
                    OptionText = "How are you doing Doltoo?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;

            case "helpdoltoo_initial":
            {
                if (source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain))
                {
                    Subject.Reply(source, "Skip", "helpdoltoo_initial2");
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.CompletedEscort))
                {
                    Subject.Reply(source, "Skip", "helpdoltoo_finish");
                    return;
                }

                if (source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart))
                {
                    Subject.Reply(source, "Skip", "helpdoltoo_return");
                }
            }
                break;

            case "helpdoltoo_initial4":
            {
                source.Trackers.Enums.Set(HelpSable.StartedDoltoo);
                var point = new Point(source.X, source.Y);
                var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brigquest");
                source.TraverseMap(mapinstance1, point);
                break;
            }

            case "helpdoltoo_return2":
            { 
                var point = new Point(source.X, source.Y);
                var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brigquest");
                source.TraverseMap(mapinstance1, point);
                source.Trackers.Enums.Set(HelpSable.StartedDoltoo);

                break;
            }

            case "helpdoltoo_finished2":
            {
                source.Trackers.Enums.Set(HelpSable.FinishedDoltoo);
                source.TryGiveGamePoints(15);
                ExperienceDistributionScript.GiveExp(source, 20000000);
                var item = ItemFactory.Create("Shinguards");
                source.GiveItemOrSendToBank(item);
                source.SendOrangeBarMessage("Doltoo hands you some Shinguards.");
                
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp from Helping Doltoo.",
                        source.Name,
                        20000000);
                break;
            }
        }
    }
}