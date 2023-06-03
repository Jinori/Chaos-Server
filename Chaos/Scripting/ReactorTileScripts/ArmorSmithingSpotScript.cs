using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class ArmorSmithingSpotScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public ArmorSmithingSpotScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (!(source is Aisling aisling))
            return;

        var blank = MerchantFactory.Create("sewing_merchant", source.MapInstance, new Point(6, 6));
        var dialog = DialogFactory.Create("armorsmithing_initial", blank);
        dialog.Display(aisling);
    }
}