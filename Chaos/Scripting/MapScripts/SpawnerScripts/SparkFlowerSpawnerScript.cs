using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class SparkFlowerSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "SparkFlower";
    public override int MaxAmount { get; set; } = 4;
    public override int MaxPerSpawn { get; set; } = 1;

    public override int SpawnChance { get; set; } = 5;
    public override int SpawnIntervalMs { get; set; } = 1800000;

    /// <inheritdoc />
    public SparkFlowerSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}