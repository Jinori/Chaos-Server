using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class StockingStufferScript : ItemScriptBase
{
    // Define a random number generator
    private static readonly Random Random = new();

    // List of items with their corresponding drop chances
    private readonly Dictionary<string, double> ItemChances = new()
    {
        {
            "hotchocolate", 0.20
        },
        {
            "babyboypenguin", 0.10
        },
        {
            "babygirlpenguin", 0.10
        },
        {
            "snowpombeanie", 0.03
        },
        {
            "snowmanbody1", 0.15
        },
        {
            "snowmanbody2", 0.15
        },
        {
            "snowmanbody3", 0.15
        },
        {
            "snowmanhat", 0.15
        },
        {
            "snowmantophat", 0.15
        },
        {
            "snowmansantahat", 0.15
        },
        {
            "snowmanpartyhat", 0.15
        },
        {
            "whitepuppy", 0.05
        },
        {
            "twinkleeyes", 0.15
        }
    };

    private readonly IItemFactory ItemFactory;

    public StockingStufferScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Total sum of all item chances
        var totalChance = ItemChances.Values.Sum();

        // Roll a random number between 0 and totalChance
        var roll = Random.NextDouble() * totalChance;

        // Determine which item the player receives based on the roll
        double cumulativeChance = 0;

        foreach (var entry in ItemChances)
        {
            cumulativeChance += entry.Value;

            if (roll <= cumulativeChance)
            {
                // Give the selected item to the player
                var item = ItemFactory.Create(entry.Key);
                source.GiveItemOrSendToBank(item);

                // Send a message to the player
                source.SendOrangeBarMessage($"You received {item.DisplayName} from the Stocking Stuffer!");

                break;
            }
        }
    }
}