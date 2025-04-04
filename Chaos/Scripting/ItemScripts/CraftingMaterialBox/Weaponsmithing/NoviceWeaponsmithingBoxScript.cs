using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Weaponsmithing;

public class NoviceWeaponsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public NoviceWeaponsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Bronze
        var bronzeAmount = Random.Next(9, 30);

        // Create items
        var bronze = _itemFactory.Create("rawbronze");

        for (var i = 0; i < bronzeAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawbronze"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(bronze);

        // Notify the player
        source.SendOrangeBarMessage($"You received {bronzeAmount + 1} Raw Bronze!");
    }
}