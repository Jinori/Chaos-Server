using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Armorsmithing;

public class InitiateArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public InitiateArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        var cottonAmount = Random.Next(4, 20); // 5-20

        var cotton = _itemFactory.Create("cotton");

        for (var i = 0; i < cottonAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("cotton"));

        source.GiveItemOrSendToBank(cotton);

        // Notify the player
        source.SendOrangeBarMessage($"You received {cottonAmount + 1} Cotton!");
    }
}