using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox;

public class InitiateArmorsmithingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public InitiateArmorsmithingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of Wool and Cotton
        var woolAmount = Random.Next(2, 7); // 3-7
        var cottonAmount = Random.Next(2, 7); // 3-7

        // Create items
        var wool = _itemFactory.Create("wool");
        var cotton = _itemFactory.Create("cotton");
        
        for (int i = 0; i < woolAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("wool"));
        }

        for (int i = 0; i < cottonAmount; i++)
        {
            source.GiveItemOrSendToBank(_itemFactory.Create("cotton"));
        }

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(wool);
        source.GiveItemOrSendToBank(cotton);

        // Notify the player
        source.SendOrangeBarMessage($"You received {woolAmount +1} Wool and {cottonAmount +1} Cotton!");
    }
}