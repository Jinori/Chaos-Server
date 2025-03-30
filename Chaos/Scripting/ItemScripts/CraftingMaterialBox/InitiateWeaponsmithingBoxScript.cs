using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox;

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

        // Generate random quantities of Mythril and Iron
        var mythrilAmount = Random.Next(2, 7); // 3-7
        var ironAmount = Random.Next(2, 7); // 3-7

        // Create items
        var mythril = _itemFactory.Create("rawmythril");
        var iron = _itemFactory.Create("rawiron");

        for (var i = 0; i < mythrilAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawmythril"));

        for (var i = 0; i < ironAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawiron"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(mythril);
        source.GiveItemOrSendToBank(iron);

        // Notify the player
        source.SendOrangeBarMessage($"You received {mythrilAmount + 1} Raw Mythril and {ironAmount + 1} Raw Iron!");
    }
}