using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Alchemy;

public class ArtisanAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public ArtisanAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Greater Monster Extract
        var greatermonsterextractAmount = Random.Next(9, 30); // 9-30

        // Create items
        var greatermonsterextract = _itemFactory.Create("greatermonsterextract");

        for (var i = 0; i < greatermonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("greatermonsterextract"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(greatermonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {greatermonsterextractAmount + 1} Greater Monster Extract!");
    }
}