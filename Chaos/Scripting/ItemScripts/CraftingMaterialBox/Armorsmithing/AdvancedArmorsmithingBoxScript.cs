using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Armorsmithing;

public class AdvancedArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public AdvancedArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        var hempAmount = Random.Next(9, 30); // 10-30

        // Create items
        var hemp = _itemFactory.Create("hemp");

        for (var i = 0; i < hempAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("hemp"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(hemp);

        // Notify the player
        source.SendOrangeBarMessage($"You received {hempAmount + 1} Hemp!");
    }
}