using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EventReactorTiles;

public class MtMerryScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public MtMerryScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
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

        if (source.StatSheet.Level < 11)
        {
            aisling.SendOrangeBarMessage("You cannot dream of Mount Merry until Level 11.");

            return;
        }

        var point = new Point(source.X, source.Y);
        var blankmerchant = MerchantFactory.Create("mtmerry_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("mtmerry_initial", blankmerchant);
        dialog.Display(aisling);
    }
}