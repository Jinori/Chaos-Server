using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox;

public class InitiateAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public InitiateAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Greater Monster Extract and Basic Monster Extract
        var greatermonsterextractAmount = Random.Next(4, 10); // 5-10
        var basicmonsterextractAmount = Random.Next(4, 10); // 5-10

        // Create items
        var greatermonsterextract = _itemFactory.Create("greatermonsterextract");
        var basicmonsterextract = _itemFactory.Create("basicmonsterextract");
        
        for (int i = 0; i < greatermonsterextractAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("greatermonsterextract"));
        }

        for (int i = 0; i < basicmonsterextractAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("basicmonsterextract"));
        }

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(greatermonsterextract);
        source.GiveItemOrSendToBank(basicmonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {greatermonsterextractAmount +1} Greater Monster Extract and {basicmonsterextractAmount +1} Basic Monster Extract!");
    }
}