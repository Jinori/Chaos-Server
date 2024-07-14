using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EingrenManor;

public class Manor3RdFloorScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public Manor3RdFloorScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3))
        {
            aisling.SendOrangeBarMessage("The stairs up are destroyed.");
            var point2 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point2);
            return;
        }
        

        if (!aisling.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner) && 
            !aisling.Trackers.Enums.HasValue(MainStoryEnums.RetryServant)) 
        {
            aisling.SendOrangeBarMessage("The stairs seems to have a seal, go speak to Goddess Miraelis");
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