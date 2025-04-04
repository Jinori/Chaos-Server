using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Alchemy;

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

        var basicmonsterextractAmount = Random.Next(9, 30); // 9-30

        var basicmonsterextract = _itemFactory.Create("basicmonsterextract");

        for (var i = 0; i < basicmonsterextractAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("basicmonsterextract"));

        source.GiveItemOrSendToBank(basicmonsterextract);

        // Notify the player
        source.SendOrangeBarMessage($"You received {basicmonsterextractAmount + 1} Basic Monster Extract!");
    }
}