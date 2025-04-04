using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Enchanting;

public class AdvancedEnchantingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();
    private readonly IItemFactory _itemFactory;

    public AdvancedEnchantingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        var totalEssences = Random.Next(24, 50);

        // List of essence types to distribute
        var essences = new[]
        {
            "essenceofaquaedon",
            "essenceofgeolith",
            "essenceofignatar",
            "essenceofmiraelis",
            "essenceofserendael",
            "essenceofskandara",
            "essenceoftheselene",
            "essenceofzephyra"
        };

        // Distribute totalEssences randomly across the available essence types
        for (var i = 0; i < totalEssences; i++)
        {
            // Pick a random essence type
            var randomEssence = essences[Random.Next(essences.Length)];

            // Create and give the item
            source.GiveItemOrSendToBank(_itemFactory.Create(randomEssence));
        }

        // Notify the player
        source.SendOrangeBarMessage($"You received {totalEssences + 1} random essences of the gods!");
    }
}