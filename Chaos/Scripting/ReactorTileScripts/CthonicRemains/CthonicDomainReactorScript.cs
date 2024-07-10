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

public class CthonicDomainReactorScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public CthonicDomainReactorScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
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

        if (aisling.Trackers.Enums.HasValue(MainStoryEnums.KilledSummoner) ||
            aisling.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
        {
            aisling.SendOrangeBarMessage("The room before you crumbled, there is no going back.");
            var point3 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point3);
            return;
        }

        if (!aisling.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2) && 
            !aisling.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)
            && !aisling.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)
            && !aisling.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight)) 
        {
            aisling.SendOrangeBarMessage("You are too afraid to venture any further.");
            var point2 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point2);
            return;
        }
        
        var point = new Point (source.X, source.Y);
        var blankmerchant = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("cthonicdomain_entrance", blankmerchant);
        dialog.Display(aisling);
    }
}