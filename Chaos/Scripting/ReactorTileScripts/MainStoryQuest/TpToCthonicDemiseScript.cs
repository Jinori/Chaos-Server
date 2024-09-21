using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class TpToCthonicDemiseScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public TpToCthonicDemiseScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
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

        if (source.IsGodModeEnabled())
        {
            var point1 = new Point (source.X, source.Y);
            var blankmerchant1 = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point1);
            var dialog1 = DialogFactory.Create("cthonicdemise_entrance", blankmerchant1);
            dialog1.Display(aisling);
        }

        if (aisling.Trackers.TimedEvents.HasActiveEvent("cthonicdemise", out _))
        {
            aisling.SendOrangeBarMessage("You have faced the army too recently, give yourself time to rest.");
            var point2 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point2);
        }
        
        if (aisling.Trackers.Flags.HasFlag(MainstoryFlags.FinishedDungeon) && !aisling.IsGodModeEnabled())
        {
            aisling.SendOrangeBarMessage("The army is gone, nothing for you there.");
            var point2 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point2);
            return;
        }
        

        if (!aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedDungeon)) 
        {
            aisling.SendOrangeBarMessage("You must speak to Goddess Miraelis before attempting this.");
            var point2 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point2);
            return;
        }
        
        var point = new Point (source.X, source.Y);
        var blankmerchant = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("cthonicdemise_entrance", blankmerchant);
        dialog.Display(aisling);
    }
}