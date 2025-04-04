using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Weaponsmithing;

public class InitiateWeaponsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public InitiateWeaponsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        var ironAmount = Random.Next(9, 30); // 10-30

        // Create items
        var mythril = _itemFactory.Create("rawmythril");

        for (var i = 0; i < ironAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawiron"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(mythril);

        // Notify the player
        source.SendOrangeBarMessage($"You received {ironAmount + 1} Raw Iron!");
    }
}