using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Alchemy;

public class NoviceAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public NoviceAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Lesser Monster Extract and Basic Monster Extract
        var lessermonsterextractAmount = Random.Next(9, 30); // 10-30

        // Create items
        var lessermonsterextract = _itemFactory.Create("lessermonsterextract");

        for (var i = 0; i < lessermonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("lessermonsterextract"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(lessermonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {lessermonsterextractAmount + 1} Lesser Monster Extract!");
    }
}