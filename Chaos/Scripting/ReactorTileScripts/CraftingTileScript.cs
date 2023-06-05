using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class CraftingTileScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public CraftingTileScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
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
        
        if (aisling?.MapInstance.InstanceId == "mileth_kitchen")
        {
            var blank = MerchantFactory.Create("stove_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("cooking_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        if (aisling?.MapInstance.InstanceId == "tagor_forge")
        {
            var blank = MerchantFactory.Create("anvil_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("weaponsmithing_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        if (aisling?.MapInstance.InstanceId == "piet_alchemy_lab")
        {
            var blank = MerchantFactory.Create("table_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("alchemy_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        
        if (aisling?.MapInstance.InstanceId == "mileth_inn")
        {
            var blank = MerchantFactory.Create("sewing_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("armorsmithing_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        if (aisling?.MapInstance.InstanceId == "undine_enchanted_haven")
        {
            var blank = MerchantFactory.Create("crystalball_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("enchanting_initial", blank);
            dialog.Display(aisling);
        }
    }
}