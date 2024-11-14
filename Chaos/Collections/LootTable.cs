using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Collections;

public sealed class LootTable(IItemFactory itemFactory) : ILootTable
{
    private const double DROPCHANCE_MULTIPLIER = 1.0;
    private readonly IItemFactory ItemFactory = itemFactory;
    public required string Key { get; init; }
    public required ICollection<LootDrop> LootDrops { get; init; } = Array.Empty<LootDrop>();
    public required LootTableMode Mode { get; init; }

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
                        yield return ItemFactory.Create(drop.ItemTemplateKey);
                }

                break;
            }
            case LootTableMode.PickSingleOrDefault:
            {
                var itemTemplateKey = LootDrops.ToDictionary(drop => drop.ItemTemplateKey, drop => drop.DropChance)
                                               .PickRandomWeightedSingleOrDefault();

                if (itemTemplateKey is not null)
                    yield return ItemFactory.Create(itemTemplateKey);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}