using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.Abel;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.HouseMacabre;

public class TeleportToCr11Script : DialogScriptBase
{

    private readonly ISimpleCache SimpleCache;
    private readonly IReactorTileFactory ReactorTileFactory;
    public TeleportToCr11Script(Dialog subject, ILogger<DecoratingInnQuestScript> logger, ISimpleCache simpleCache, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        ReactorTileFactory = reactorTileFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out MainStoryEnums stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "hazel_initial":
            {
                
                var option = new DialogOption
                {
                    DialogKey = "teleportcr11_initial",
                    OptionText = "Open Portal"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "teleportcr11_initial":
            {
                if (hasStage && stage == MainStoryEnums.CompletedPreMasterMainStory)
                {
                    Subject.Reply(source, "Skip", "teleportcr11_initial1");
                }

                break;
            }

            case "teleportcr11_initial4":
            {
                if (!source.TryTakeGold(250000))
                {
                    Subject.Reply(source, "You don't have 250,000 Gold to give Hazel.");
                    return;
                }
                
                Subject.Close(source);
                var point = new Point(3, 5);
                var portal = ReactorTileFactory.Create("cr11portal", source.MapInstance, point, null, source);
                source.MapInstance.SimpleAdd(portal);

                if (source.Group != null)
                    foreach (var aisling in source.Group)
                    {
                        aisling.SendOrangeBarMessage($"{source.Name} opens a portal to Cthonic Remains 11.");
                    }
                break;
            }
        }
    }
}