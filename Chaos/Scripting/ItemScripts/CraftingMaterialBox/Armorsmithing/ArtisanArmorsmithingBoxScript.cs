using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Armorsmithing;

public class ArtisanArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public ArtisanArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Wool and Silk
        var woolAmount = Random.Next(4, 20); // 5-20

        // Create items
        var wool = _itemFactory.Create("wool");

        for (var i = 0; i < woolAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("wool"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(wool);

        // Notify the player
        source.SendOrangeBarMessage($"You received {woolAmount + 1} Wool!");
    }
}