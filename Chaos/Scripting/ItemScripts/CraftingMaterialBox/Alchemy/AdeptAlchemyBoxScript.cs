using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Alchemy;

public class AdeptAlchemyBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public AdeptAlchemyBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities Superior Monster Extract
        var superiormonsterextractAmount = Random.Next(9, 30); // 10-30

        var superiormonsterextract = _itemFactory.Create("superiormonsterextract");

        for (var i = 0; i < superiormonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("superiormonsterextract"));

        source.GiveItemOrSendToBank(superiormonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {superiormonsterextractAmount + 1} Superior Monster Extract!");
    }
}