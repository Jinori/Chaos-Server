using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Armorsmithing;

public class NoviceArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public NoviceArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Linen and Cotton
        var linenAmount = Random.Next(4, 20); // 5-20

        // Create items
        var linen = _itemFactory.Create("linen");

        for (var i = 0; i < linenAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("linen"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(linen);

        // Notify the player
        source.SendOrangeBarMessage($"You received {linenAmount + 1} Linen!");
    }
}