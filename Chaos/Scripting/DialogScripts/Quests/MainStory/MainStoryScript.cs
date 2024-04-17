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
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStoryScript(
    Dialog subject,
    IItemFactory itemFactory,
    ILogger<SpareAStickScript> logger,
    IMerchantFactory merchantFactory,
    IDialogFactory dialogFactory)
    : DialogScriptBase(subject)
{
    private readonly ILogger<SpareAStickScript> Logger = logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
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
                    
                    var option = new DialogOption
                    {
                        DialogKey = "mysteriousartifact_start6",
                        OptionText = "Speak the words of Zephyr"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }
                break;

            case "zephyr_start1":
            {
                var point = new Point(source.X, source.Y);
                var zephyr = merchantFactory.Create("zephyr", source.MapInstance, point);
            }
                break;
        }
    }
}