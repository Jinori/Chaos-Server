using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class PassionFlowerSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "passionflower";
    public override int MaxAmount { get; set; } = 4;
    public override int MaxPerSpawn { get; set; } = 1;

    public override int SpawnChance { get; set; } = 2;
    public override int SpawnIntervalMs { get; set; } = 2000000;

    /// <inheritdoc />
    public PassionFlowerSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}