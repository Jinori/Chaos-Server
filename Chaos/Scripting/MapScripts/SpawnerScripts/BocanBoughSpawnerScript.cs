using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class BocanBoughSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "bocanbough";
    public override int MaxAmount { get; set; } = 4;
    public override int MaxPerSpawn { get; set; } = 2;
    public override int SpawnIntervalMs { get; set; } = 1000000;
    public override int SpawnChance { get; set; } = 15;

    /// <inheritdoc />
    public BocanBoughSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}