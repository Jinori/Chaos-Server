using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Alchemy;

public class AdvancedAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public AdvancedAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Prime Monster Extract and Superior Monster Extract
        var primemonsterextractAmount = Random.Next(4, 10); // 5-10
        var superiormonsterextractAmount = Random.Next(9, 20); // 10-20

        // Create items
        var primemonsterextract = _itemFactory.Create("primemonsterextract");
        var superiormonsterextract = _itemFactory.Create("superiormonsterextract");

        for (var i = 0; i < primemonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("primemonsterextract"));

        for (var i = 0; i < superiormonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("superiormonsterextract"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(primemonsterextract);
        source.GiveItemOrSendToBank(superiormonsterextract);

        // Notify the player
        source.SendOrangeBarMessage(
            $"You received {primemonsterextractAmount + 1} Greater Monster Extract and {superiormonsterextractAmount + 1} Superior Monster Extract!");
    }
}