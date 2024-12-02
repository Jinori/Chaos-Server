using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class BasicAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public BasicAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Lesser Monster Extract and Basic Monster Extract
        var lessermonsterextractAmount = Random.Next(4, 10); // 5-10
        var basicmonsterextractAmount = Random.Next(4, 10); // 5-10

        // Create items
        var lessermonsterextract = _itemFactory.Create("lessermonsterextract");
        var basicmonsterextract = _itemFactory.Create("basicmonsterextract");
        
        for (int i = 0; i < lessermonsterextractAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("lessermonsterextract"));
        }

        for (int i = 0; i < basicmonsterextractAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("basicmonsterextract"));
        }

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(lessermonsterextract);
        source.GiveItemOrSendToBank(basicmonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {lessermonsterextractAmount +1} Lesser Monster Extract and {basicmonsterextractAmount +1} Basic Monster Extract!");
    }
}