using Chaos.Definitions;
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
     
        var hasStage = aisling.Trackers.Enums.TryGetValue(out Crafts stage);
        
        if (aisling?.MapInstance.InstanceId == "mileth_kitchen")
        {
            if (!aisling.IsAdmin)
            {
                if (!aisling.Trackers.Flags.HasFlag(Hobbies.Cooking))
                {
                    aisling.SendOrangeBarMessage("You are not a chef.");

                    return;
                }
            }


            var blank = MerchantFactory.Create("stove_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("cooking_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        if (aisling?.MapInstance.InstanceId == "tagor_forge")
        {
            if (!aisling.IsAdmin)
            {
                if (!hasStage || (stage != Crafts.Weaponsmithing))
                {
                    aisling.SendOrangeBarMessage("You are not a Weaponsmith.");

                    return;
                }
            }

            var blank = MerchantFactory.Create("anvil_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("weaponsmithing_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        if (aisling?.MapInstance.InstanceId == "piet_alchemy_lab")
        {
            if (!aisling.IsAdmin)
            {
                if (!hasStage || (stage == Crafts.Alchemy))
                {
                    aisling.SendOrangeBarMessage("You are not a Alchemist.");

                    return;
                }
            }
            var blank = MerchantFactory.Create("table_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("alchemy_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        
        if (aisling?.MapInstance.InstanceId == "mileth_inn")
        {
            if (!aisling.IsAdmin)
            {
                if (!hasStage || (stage == Crafts.Armorsmithing))
                {
                    aisling.SendOrangeBarMessage("You are not an Armorsmith.");

                    return;
                }
            }

            var blank = MerchantFactory.Create("sewing_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("armorsmithing_initial", blank);
            dialog.Display(aisling);
            
            return;
        }
        if (aisling?.MapInstance.InstanceId == "undine_enchanted_haven")
        {
            if (!aisling.IsAdmin)
            {
                if (!hasStage || (stage == Crafts.Enchanting))
                {
                    aisling.SendOrangeBarMessage("You are not an Enchanter.");

                    return;
                }
            }

            var blank = MerchantFactory.Create("crystalball_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("enchanting_initial", blank);
            dialog.Display(aisling);
            return;
        }
        
        if (aisling?.MapInstance.InstanceId == "rucesion_jeweler")
        {
            if (!aisling.IsAdmin)
            {
                if (!hasStage || (stage == Crafts.Jewelcrafting))
                {
                    aisling.SendOrangeBarMessage("You are not a Jeweler.");

                    return;
                }
            }

            var blank = MerchantFactory.Create("jewelerbench_merchant", source.MapInstance, new Point(6, 6));
            var dialog = DialogFactory.Create("jewelcrafting_initial", blank);
            dialog.Display(aisling);
        }
    }
}