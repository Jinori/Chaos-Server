using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Mileth;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStoryScript(
    Dialog subject,
    IItemFactory itemFactory,
    ILogger<SpareAStickScript> logger,
    IMerchantFactory merchantFactory,
    ISimpleCache simpleCache,
    IDialogFactory dialogFactory)
    : DialogScriptBase(subject)
{
    private readonly ILogger<SpareAStickScript> Logger = logger;

    private readonly ISimpleCache SimpleCache = simpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } =
        DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        if (source.MapInstance.Name.Contains("The God's Realm"))
        {
            Subject.Reply(source, "You are in the presence of the Gods, do not be rude.");
            return;
        }
        var hasStage = source.Trackers.Enums.TryGetValue(out MainStoryEnums stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "mysteriousartifact_yes":
            {

                if (hasStage && stage == MainStoryEnums.MysteriousArtifactFound)
                {
                    var mysteriousartifact = itemFactory.Create("mysteriousartifact");
                    source.TryGiveItems(mysteriousartifact);
                    source.Trackers.Enums.Set(MainStoryEnums.ReceivedMA);
                }

                break;
            }

            case "mysteriousartifact_initial1":
            {
                if (source.Trackers.Enums.HasValue(MainStoryEnums.ReceivedMA))
                {
                    if (source.UserStatSheet.Level < 11)
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage("You are too inexperienced to interact with this object.");
                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "mysteriousartifact_start1",
                        OptionText = "Listen closely to the Artifact"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    if (source.UserStatSheet.Level < 41)
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage("You are not ready to face the Gods.");
                        return;
                    }
                    
                    Subject.Reply(source, "Skip", "mysteriousartifact_initial2");
                    return;
                }
            }
                break;
            case "zephyr_start1":
            {
                var point = new Point(source.X, source.Y);
                var zephyr = merchantFactory.Create("zephyr", source.MapInstance, point);
                var zephyrDialog = dialogFactory.Create("zephyr_initial", zephyr);
                zephyrDialog.Display(source);
            }
                break;

            case "zephyr_initial5":
            {
                source.Trackers.Enums.Set(MainStoryEnums.SpokeToZephyr);
                return;
            }
                break;
            
            case "mysteriousartifact_start3":
            {
                Subject.Close(source);
                var godsrealm = SimpleCache.Get<MapInstance>("godsrealm");
                var newPoint = new Point(16, 16);
                
                source.TraverseMap(godsrealm, newPoint);
            }
                break;
        }
    }
}