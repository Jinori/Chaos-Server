using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class TpToCreantScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public TpToCreantScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
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
            var point1 = new Point(source.X, source.Y);
            var blankmerchant1 = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point1);
            var dialog1 = DialogFactory.Create("creant_entrance", blankmerchant1);
            dialog1.Display(aisling);
        }

        if (aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.CompletedCreants) && !aisling.IsGodModeEnabled())
        {
            aisling.SendOrangeBarMessage("The Creant is dead, the altar does nothing.");

            return;
        }

        if (!aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.KilledCreants))
        {
            aisling.SendOrangeBarMessage("All of the Creants have been killed. Speak to Goddess Miraelis.");

            return;
        }

        if (!aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
        {
            aisling.SendOrangeBarMessage("You must speak to Goddess Miraelis before attempting this.");

            return;
        }

        var point = new Point(source.X, source.Y);
        var blankmerchant = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("creant_entrance", blankmerchant);
        dialog.Display(aisling);
    }
}