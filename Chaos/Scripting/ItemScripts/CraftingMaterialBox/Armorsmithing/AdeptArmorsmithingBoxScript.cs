using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Armorsmithing;

public class AdeptArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public AdeptArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        var silkAmount = Random.Next(4, 20); // 5-20

        var silk = _itemFactory.Create("silk");

        for (var i = 0; i < silkAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("silk"));

        source.GiveItemOrSendToBank(silk);

        // Notify the player
        source.SendOrangeBarMessage($"You received {silkAmount + 1} Silk!");
    }
}