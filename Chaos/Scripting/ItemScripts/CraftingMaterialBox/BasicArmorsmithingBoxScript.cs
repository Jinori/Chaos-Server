using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class BasicArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public BasicArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Linen and Cotton
        var linenAmount = Random.Next(2, 7); // 3-7
        var cottonAmount = Random.Next(2, 7); // 3-7

        // Create items
        var linen = _itemFactory.Create("linen");
        var cotton = _itemFactory.Create("cotton");

        for (var i = 0; i < linenAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("linen"));

        for (var i = 0; i < cottonAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("cotton"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(linen);
        source.GiveItemOrSendToBank(cotton);

        // Notify the player
        source.SendOrangeBarMessage($"You received {linenAmount + 1} Linen and {cottonAmount + 1} Cotton!");
    }
}