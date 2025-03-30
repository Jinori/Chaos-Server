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
    private readonly IReactorTileFactory ReactorTileFactory;

    private readonly ISimpleCache SimpleCache;

    public TeleportToCr11Script(
        Dialog subject,
        ILogger<DecoratingInnQuestScript> logger,
        ISimpleCache simpleCache,
        IReactorTileFactory reactorTileFactory)
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
                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.FinishedDungeon))
                    if (!source.Trackers.Flags.HasFlag(CdDungeonBoss.CompletedDungeonOnce))
                        source.Trackers.Flags.AddFlag(CdDungeonBoss.CompletedDungeonOnce);

                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.CompletedDungeonOnce))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "teleportcr21_initial",
                        OptionText = "Open Portal CR21"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (hasStage && (stage == MainStoryEnums.CompletedPreMasterMainStory))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "teleportcr11_initial",
                        OptionText = "Open Portal CR11"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "teleportcr11_initial":
            {
                if (source.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
                    Subject.Reply(source, "Skip", "teleportcr11_initial1");

                break;
            }

            case "teleportcr11_initial4":
            {
                if (!source.TryTakeGold(200000))
                {
                    Subject.Reply(source, "You don't have 200,000 Gold to give Hazel.");

                    return;
                }

                Subject.Close(source);
                var point = new Point(3, 5);

                var portal = ReactorTileFactory.Create(
                    "cr11portal",
                    source.MapInstance,
                    point,
                    null,
                    source);
                source.MapInstance.SimpleAdd(portal);

                if (source.Group != null)
                    foreach (var aisling in source.Group)
                        aisling.SendOrangeBarMessage($"{source.Name} opens a portal to Cthonic Remains 11.");

                break;
            }

            case "teleportcr21_initial":
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.CompletedDungeonOnce))
                    Subject.Reply(source, "Skip", "teleportcr21_initial1");

                break;
            }

            case "teleportcr21_initial4":
            {
                if (!source.TryTakeGold(400000))
                {
                    Subject.Reply(source, "You don't have 400,000 Gold to give Hazel.");

                    return;
                }

                Subject.Close(source);
                var point = new Point(3, 8);

                var portal = ReactorTileFactory.Create(
                    "cr21portal",
                    source.MapInstance,
                    point,
                    null,
                    source);
                source.MapInstance.SimpleAdd(portal);

                if (source.Group != null)
                    foreach (var aisling in source.Group)
                        aisling.SendOrangeBarMessage($"{source.Name} opens a portal to Cthonic Remains 21.");

                break;
            }
        }
    }
}