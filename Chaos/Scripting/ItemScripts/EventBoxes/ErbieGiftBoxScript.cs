using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class ErbieGiftBoxScript : ItemScriptBase
{
    // Define a random number generator
    private static readonly Random Random = new();

    // List of items with their corresponding drop chances
    private readonly Dictionary<string, double> ItemChances = new()
    {
        {
            "angrysnowman", 0.15
        },
        {
            "erbiebackpack", 0.05
        },
        {
            "erbiehat", 0.08
        },
        {
            "snowmanbuddy", 0.08
        },
        {
            "peterbie", 0.03
        },
        {
            "petholidayerbie", 0.03
        },
        {
            "ranchdress", 0.10
        },
        {
            "ranchsuit", 0.10
        },
        {
            "ranchhat", 0.10
        },
        {
            "ranchbow", 0.10
        },
        {
            "redantlers", 0.15
        },
        {
            "fskioutfit", 0.05
        },
        {
            "mskisuit", 0.05
        },
        {
            "winterscarf", 0.05
        }
    };

    private readonly IItemFactory ItemFactory;

    public ErbieGiftBoxScript(Item subject, IItemFactory itemFactory)
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
                source.SendOrangeBarMessage($"You received {item.DisplayName} from the Erbie Gift Box!");

                break;
            }
        }
    }
}