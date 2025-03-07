﻿using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class MountMerryBoxScript : ItemScriptBase
{
    // Define a random number generator
    private static readonly Random Random = new();

    // List of items with their corresponding drop chances
    private readonly Dictionary<string, double> ItemChances = new()
    {
        {
            "coldstreamdress", 0.15
        },
        {
            "coldstreamsuit", 0.15
        },
        {
            "mcoldstreamhat", 0.15
        },
        {
            "fcoldstreamhat", 0.15
        },
        {
            "reindeercostume", 0.15
        },
        {
            "reindeerhat", 0.15
        },
        {
            "santahead", 0.05
        },
        {
            "snowbeastbuddy", 0.03
        },
        {
            "holidaybow", 0.10
        },
        {
            "holidaydress", 0.10
        },
        {
            "holidaypartyhat", 0.15
        },
        {
            "holidayrobe", 0.08
        },
        {
            "holidayscarf", 0.05
        },
        {
            "holidayspecs", 0.05
        },
        {
            "mholidayband", 0.08
        },
        {
            "fholidayband", 0.15
        }
    };

    private readonly IItemFactory ItemFactory;

    public MountMerryBoxScript(Item subject, IItemFactory itemFactory)
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
                source.SendOrangeBarMessage($"You received {item.DisplayName} from the Mount Merry Box!");

                break;
            }
        }
    }
}