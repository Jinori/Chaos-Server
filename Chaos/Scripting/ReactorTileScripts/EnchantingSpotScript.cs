using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class EnchantingSpotScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public EnchantingSpotScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var blank = MerchantFactory.Create("enchant_merchant", source.MapInstance, new Point(5, 12));
        var dialog = DialogFactory.Create("enchanting_initial", blank);
        dialog.Display(aisling);
    }
}