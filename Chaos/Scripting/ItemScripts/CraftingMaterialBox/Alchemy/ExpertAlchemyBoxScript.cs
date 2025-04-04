using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Alchemy;

public class ExpertAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public ExpertAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Greater Monster Extract and Superior Monster Extract
        var primemonsterextractAmount = Random.Next(10, 30); // 5-10

        // Create items
        var primemonsterextract = _itemFactory.Create("primemonsterextract");

        for (var i = 0; i < primemonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("primemonsterextract"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(primemonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {primemonsterextractAmount + 1} Prime Monster Extract!");
    }
}