using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox;

public class BasicWeaponsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public BasicWeaponsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Bronze and Iron
        var bronzeAmount = Random.Next(2, 7); // 3-7
        var ironAmount = Random.Next(2, 7); // 3-7

        // Create items
        var bronze = _itemFactory.Create("rawbronze");
        var iron = _itemFactory.Create("rawiron");

        for (var i = 0; i < bronzeAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawbronze"));

        for (var i = 0; i < ironAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawiron"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(bronze);
        source.GiveItemOrSendToBank(iron);

        // Notify the player
        source.SendOrangeBarMessage($"You received {bronzeAmount + 1} Raw Bronze and {ironAmount + 1} Raw Iron!");
    }
}