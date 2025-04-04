using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Weaponsmithing;

public class AdvancedWeaponsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public AdvancedWeaponsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Mythril and Hy-Brasyl
        var azuriumAmount = Random.Next(4, 15); // 3-7
        var crimsoniteAmount = Random.Next(4, 15); // 3-7

        // Create items
        var azurium = _itemFactory.Create("rawazurium");
        var crimsonite = _itemFactory.Create("rawcrimsonite");

        for (var i = 0; i < azuriumAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawazurium"));

        for (var i = 0; i < crimsoniteAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawcrimsonite"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(azurium);
        source.GiveItemOrSendToBank(crimsonite);

        // Notify the player
        source.SendOrangeBarMessage($"You received {crimsoniteAmount + 1} Raw Crimsonite and {azuriumAmount + 1} Raw Azurium!");
    }
}