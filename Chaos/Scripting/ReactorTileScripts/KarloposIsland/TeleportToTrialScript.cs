using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.KarloposIsland;

public class TeleportToTrialScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public TeleportToTrialScript(
        ReactorTile subject,
        IItemFactory itemFactory,
        IDialogFactory dialogFactory,
        ISimpleCache simpleCache)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var currentMap = SimpleCache.Get<MapInstance>(source.MapInstance.InstanceId);
        var aisling = source as Aisling;

        var monstersOnMap = currentMap.GetEntities<Monster>()
                                      .Where(entity => !entity.Template.TemplateKey.Contains("Pet"));

        if (monstersOnMap.Any())
        {
            aisling?.SendOrangeBarMessage("There are still monsters lurking.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        if ((currentMap.Name != "Karlopos Island Long Beach") && (currentMap.Name != "Karlopos Secret Passage"))
        {
            if (aisling is { Group: not null })
            {
                if (!aisling.Trackers.Counters.TryGetValue("karlopostrialkills", out var killcount) || (killcount < 1))
                {
                    // If any player has less than 5 kills, send a message and warp them back
                    aisling?.SendOrangeBarMessage("You haven't defeated the trial.");
                    var point = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(point);

                    return;
                }
            } else
                aisling?.SendOrangeBarMessage("Your group is not near.");
        }

        var item = ItemFactory.Create("redpearl");
        var classDialog = DialogFactory.Create("queenoctopus_trial", item);

        if (aisling != null)
            classDialog.Display(aisling);
    }
}