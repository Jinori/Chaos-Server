using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class MrHoppsBoxScript : ItemScriptBase
{
    // Define a random number generator
    private static readonly Random Random = new();

    // List of items with their corresponding drop chances
    private readonly Dictionary<string, double> ItemChances = new()
    {
        {
            "bunnycostume", 0.15
        },
        {
            "bunnybackpack", 0.05
        },
        {
            "bunnyfairy", 0.08
        },
        {
            "bunnyears", 0.03
        },
        {
            "femalebunnyoutfit", 0.03
        },
        {
            "bunnypaws", 0.10
        },
        {
            "bunnyfeet", 0.10
        },
        {
            "bunnytail", 0.10
        },
        {
            "bunnyhead", 0.10
        },
        {
            "male_bunnytophat", 0.05
        },
        {
            "female_bluebow", 0.05
        },
        {
            "bunnypolyp", 0.05
        },
        {
            "scarletkimonodress", 0.02
        },
        {
            "indigokimonodress", 0.02
        },
        {
            "pinkkimonodress", 0.02
        },
        {
            "cobaltkimonorobe", 0.02
        },
        {
            "slatekimonorobe", 0.02
        },
        {
            "crimsonkimonorobe", 0.02
        }
    };

    private readonly IItemFactory ItemFactory;

    public MrHoppsBoxScript(Item subject, IItemFactory itemFactory)
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
                source.SendOrangeBarMessage($"You received {item.DisplayName} from Mr.Hopps's Box");

                break;
            }
        }
    }
}