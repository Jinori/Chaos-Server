#region
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a table of loot from which items can be generated
/// </summary>
/// <param name="itemFactory">
///     A service used to create items
/// </param>
public sealed class LootTable(IItemFactory itemFactory) : ILootTable
{
    private const double DROPCHANCE_MULTIPLIER = 1.0;
    private const double SERENDAEL_MULTIPLIER = 1.25;
    private readonly IItemFactory ItemFactory = itemFactory;

    /// <summary>
    ///     A unique string key identifier for the loot table
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    ///     The loot drops that can be generated from the table
    /// </summary>
    public required ICollection<LootDrop> LootDrops { get; init; } = Array.Empty<LootDrop>();

    /// <summary>
    ///     The mode in which the loot table operates
    /// </summary>
    public required LootTableMode Mode { get; init; }

    /// <inheritdoc />
    public IEnumerable<Item> GenerateLoot()
    {
        switch (Mode)
        {
            case LootTableMode.ChancePerItem:
            {
                foreach (var drop in LootDrops)
                {
                    var adjustedDropChance = drop.DropChance * (decimal)DROPCHANCE_MULTIPLIER;

                    if (DecimalRandomizer.RollChance(adjustedDropChance))
                        yield return ItemFactory.Create(drop.ItemTemplateKey, drop.ExtraScriptKeys);
                }

                break;
            }
            case LootTableMode.PickSingleOrDefault:
            {
                var drop = LootDrops.ToDictionary(drop => drop, drop => drop.DropChance)
                                    .PickRandomWeightedSingleOrDefault();

                if (drop is not null)
                    yield return ItemFactory.Create(drop.ItemTemplateKey, drop.ExtraScriptKeys);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <inheritdoc />
    public IEnumerable<Item> GenerateLoot(bool hasSerendaelBuff)
    {
        var dropMultiplier = hasSerendaelBuff ? SERENDAEL_MULTIPLIER : DROPCHANCE_MULTIPLIER;

        switch (Mode)
        {
            case LootTableMode.ChancePerItem:
            {
                foreach (var drop in LootDrops)
                {
                    var adjustedDropChance = drop.DropChance * (decimal)dropMultiplier;

                    if (DecimalRandomizer.RollChance(adjustedDropChance))
                        yield return ItemFactory.Create(drop.ItemTemplateKey);
                }

                break;
            }
            case LootTableMode.PickSingleOrDefault:
            {
                var weightedDrops = LootDrops.ToDictionary(
                    drop => drop.ItemTemplateKey, 
                    drop => drop.DropChance * (decimal)dropMultiplier
                );

                var itemTemplateKey = weightedDrops.PickRandomWeightedSingleOrDefault();

                if (itemTemplateKey is not null)
                    yield return ItemFactory.Create(itemTemplateKey);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}