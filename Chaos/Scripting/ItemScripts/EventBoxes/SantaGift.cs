using Chaos.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class SantaGift : ItemScriptBase
{
    // Define a random number generator
    private static readonly Random Random = new();

    // List of items with their corresponding drop chances
    private readonly Dictionary<string, double> ItemChances = new()
    {
        {
            "hotchocolate", 0.15
        },
        {
            "snowpombeanie", 0.05
        },
        {
            "whitepuppy", 0.08
        },
        {
            "winterscarf", 0.08
        },
        {
            "fskioutfit", 0.08
        },
        {
            "mskioutfit", 0.08
        },
        {
            "peterbie", 0.05
        },
        {
            "erbiebackpack", 0.08
        },
        {
            "erbiehat", 0.1
        },
        {
            "musiceffect", 0.05
        },
        {
            "petpenguin", 0.05
        },
        {
            "whitebeard", 0.15
        },
        {
            "holidayscarf", 0.08
        },
        {
            "holidayspecs", 0.08
        },
        {
            "mholidayband", 0.08
        },
        {
            "fholidayband", 0.15
        },
        {
            "whitewinterscarf", 0.05
        },
        {
            "snowbeastbuddy", 0.05
        },
        {
            "recipe_hotchocolate", 0.03
        },
        {
            "dunanmount", 0.01
        },
        {
            "yuletree", 0.02
        },
        {
            "koboldwhacker", 0.02
        }
    };

    private readonly IItemFactory ItemFactory;

    public SantaGift(Item subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Total sum of all item chances
        var totalChance = ItemChances.Values.Sum();

        while (true)
        {
            // Roll a random number between 0 and totalChance
            var roll = Random.NextDouble() * totalChance;

            // Determine which item the player receives based on the roll
            double cumulativeChance = 0;

            foreach (var entry in ItemChances)
            {
                cumulativeChance += entry.Value;

                if (roll <= cumulativeChance)
                {
                    // If the item is "dunanmount" and the player already owns it, reroll
                    if ((entry.Key == "dunanmount") && source.Trackers.Flags.HasFlag(AvailableMounts.Dunan))
                        break;

                    // Give the selected item to the player
                    var item = ItemFactory.Create(entry.Key);
                    source.GiveItemOrSendToBank(item);

                    // Send a message to the player
                    source.SendOrangeBarMessage($"You received {item.DisplayName} from Santa's Gift!");

                    return; // Exit the method after successfully giving an item
                }
            }
        }
    }
}