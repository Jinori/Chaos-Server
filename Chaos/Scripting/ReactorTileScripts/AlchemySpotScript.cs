using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class AlchemySpotScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public AlchemySpotScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var blank = MerchantFactory.Create("table_merchant", source.MapInstance, new Point(6, 6));
        var dialog = DialogFactory.Create("alchemy_initial", blank);
        dialog.Display(aisling);
    }
}