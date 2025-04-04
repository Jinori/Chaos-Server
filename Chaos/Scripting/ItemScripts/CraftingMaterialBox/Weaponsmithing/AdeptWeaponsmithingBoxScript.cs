using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Weaponsmithing;

public class AdeptWeaponsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public AdeptWeaponsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities Hy-Brasyl
        var hybrasylAmount = Random.Next(9, 30); // 3-7

        var hybrasyl = _itemFactory.Create("rawhybrasyl");

        for (var i = 0; i < hybrasylAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawhybrasyl"));

        source.GiveItemOrSendToBank(hybrasyl);

        // Notify the player
        source.SendOrangeBarMessage($"You received {hybrasylAmount + 1} Raw Hy-Brasyl!");
    }
}