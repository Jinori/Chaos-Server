using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox;

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

        // Generate random quantities of Greater Monster Extract and Superior Monster Extract
        var greatermonsterextractAmount = Random.Next(4, 10); // 5-10
        var superiormonsterextractAmount = Random.Next(4, 10); // 5-10

        // Create items
        var greatermonsterextract = _itemFactory.Create("greatermonsterextract");
        var superiormonsterextract = _itemFactory.Create("superiormonsterextract");
        
        for (int i = 0; i < greatermonsterextractAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("greatermonsterextract"));
        }

        for (int i = 0; i < superiormonsterextractAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("superiormonsterextract"));
        }

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(greatermonsterextract);
        source.GiveItemOrSendToBank(superiormonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {greatermonsterextractAmount +1} Greater Monster Extract and {superiormonsterextractAmount +1} Superior Monster Extract!");
    }
}