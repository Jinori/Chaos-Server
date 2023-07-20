using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class RefiningTileScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public RefiningTileScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (Subject.MapInstance.InstanceId == "tagor_forge")
        {
            var merchant = MerchantFactory.Create("metal_refining_merchant", source.MapInstance, new Point(9, 2));
            var dialog = DialogFactory.Create("metal_refining_initial", merchant);
            dialog.Display(aisling);
        }
        
        if (Subject.MapInstance.InstanceId == "rucesion_jeweler")
        {
            var merchant = MerchantFactory.Create("gem_refining_merchant", source.MapInstance, new Point(9, 2));
            var dialog = DialogFactory.Create("gem_refining_initial", merchant);
            dialog.Display(aisling);
        }
        if (Subject.MapInstance.InstanceId == "wilderness_armorsmithing")
        {
            var merchant = MerchantFactory.Create("fabric_refining_merchant", source.MapInstance, new Point(9, 2));
            var dialog = DialogFactory.Create("fabric_refining_initial", merchant);
            dialog.Display(aisling);
        }
    }
}