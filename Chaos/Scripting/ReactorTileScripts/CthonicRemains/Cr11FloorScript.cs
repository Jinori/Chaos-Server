using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.CthonicRemains;

public class Cr11FloorScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public Cr11FloorScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.Enums.HasValue(MainStoryEnums.KilledSummoner) ||
            aisling.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
        {
            var mapinstance = SimpleCache.Get<MapInstance>("cr11");
            var point1 = new Point(18, 46);
            aisling.TraverseMap(mapinstance, point1);
        }

        if (!aisling.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2) && 
            !aisling.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)
            && !aisling.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)) 
        {
            aisling.SendOrangeBarMessage("You are too afraid to venture any further.");
            var point2 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point2);
            return;
        }
        
        var point = new Point (source.X, source.Y);
        var blankmerchant = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("3rdfloor_entrance", blankmerchant);
        dialog.Display(aisling);
    }
}