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
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public TeleportToTrialScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory, ISimpleCache simpleCache)
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
        
        if (currentMap.GetEntities<Monster>().Any())
        {
            aisling?.SendOrangeBarMessage("There are still monsters lurking.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
            return;
        }
        
        var item = ItemFactory.Create("redpearl");
        var classDialog = DialogFactory.Create("queenoctopus_trial", item);

        if (aisling != null) classDialog.Display(aisling);
    }
}